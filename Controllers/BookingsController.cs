using CampsiteBooking.Infrastructure.Kafka;
using CampsiteBooking.Infrastructure.Security;
using CampsiteBooking.Models;
using CampsiteBooking.Models.Common;
using CampsiteBooking.Models.DomainEvents;
using CampsiteBooking.Models.Repositories;
using CampsiteBooking.Models.ValueObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampsiteBooking.Controllers;

/// <summary>
/// REST API Controller for Booking CRUD operations.
/// Implements REST maturity level 2 (HTTP verbs + status codes).
/// Protected with JWT authentication.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize] // Require JWT authentication for all endpoints
public class BookingsController : ControllerBase
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IKafkaProducer _kafkaProducer;
    private readonly ILogger<BookingsController> _logger;

    public BookingsController(
        IBookingRepository bookingRepository,
        IKafkaProducer kafkaProducer,
        ILogger<BookingsController> logger)
    {
        _bookingRepository = bookingRepository ?? throw new ArgumentNullException(nameof(bookingRepository));
        _kafkaProducer = kafkaProducer ?? throw new ArgumentNullException(nameof(kafkaProducer));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// GET /api/bookings - Get all bookings
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<BookingDto>>> GetAll(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all bookings");
        var bookings = await _bookingRepository.GetAllAsync(cancellationToken);
        var dtos = bookings.Select(MapToDto);
        return Ok(dtos);
    }

    /// <summary>
    /// GET /api/bookings/{id} - Get booking by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookingDto>> GetById(int id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting booking {BookingId}", id);
        
        var bookingId = BookingId.Create(id);
        var booking = await _bookingRepository.GetByIdAsync(bookingId, cancellationToken);

        if (booking == null)
        {
            _logger.LogWarning("Booking {BookingId} not found", id);
            return NotFound(new { message = $"Booking with ID {id} not found" });
        }

        return Ok(MapToDto(booking));
    }

    /// <summary>
    /// POST /api/bookings - Create new booking
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BookingDto>> Create(
        [FromBody] CreateBookingRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Creating booking for Guest {GuestId}", request.GuestId);

            // XSS Protection: Validate special requests input
            if (!string.IsNullOrWhiteSpace(request.SpecialRequests))
            {
                InputValidator.ValidateForXss(request.SpecialRequests, "SpecialRequests");
                InputValidator.ValidateLength(request.SpecialRequests, "SpecialRequests", 1000);
            }

            // Convert to Value Objects
            var guestId = GuestId.Create(request.GuestId);
            var campsiteId = CampsiteId.Create(request.CampsiteId);
            var accommodationTypeId = AccommodationTypeId.Create(request.AccommodationTypeId);
            var period = DateRange.Create(request.CheckInDate, request.CheckOutDate);
            var basePrice = Money.Create(request.BasePriceAmount, request.Currency ?? "DKK");

            // Use domain factory method
            var booking = Booking.Create(
                guestId,
                campsiteId,
                accommodationTypeId,
                period,
                basePrice,
                request.NumberOfAdults,
                request.NumberOfChildren,
                request.SpecialRequests ?? string.Empty);

            // Save to database
            await _bookingRepository.AddAsync(booking, cancellationToken);
            await _bookingRepository.SaveChangesAsync(cancellationToken);

            // Publish domain events to Kafka (manual - simple approach)
            var events = booking.GetDomainEvents();
            foreach (var domainEvent in events)
            {
                await _kafkaProducer.PublishAsync(domainEvent, cancellationToken);
            }
            booking.ClearDomainEvents();

            _logger.LogInformation("Booking {BookingId} created successfully", booking.Id.Value);

            return CreatedAtAction(
                nameof(GetById),
                new { id = booking.Id.Value },
                MapToDto(booking));
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain validation failed");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating booking");
            return BadRequest(new { message = "An error occurred while creating the booking" });
        }
    }

    /// <summary>
    /// PUT /api/bookings/{id} - Update existing booking
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BookingDto>> Update(
        int id,
        [FromBody] UpdateBookingRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Updating booking {BookingId}", id);

            var bookingId = BookingId.Create(id);
            var booking = await _bookingRepository.GetByIdAsync(bookingId, cancellationToken);

            if (booking == null)
            {
                _logger.LogWarning("Booking {BookingId} not found", id);
                return NotFound(new { message = $"Booking with ID {id} not found" });
            }

            // Update booking - only special requests can be updated (simple for 3. semester)
            // For full update, would need to add UpdatePeriod() and UpdateGuestCount() to domain model
            if (!string.IsNullOrWhiteSpace(request.SpecialRequests))
            {
                booking.UpdateSpecialRequests(request.SpecialRequests);
            }

            // Save changes
            await _bookingRepository.UpdateAsync(booking, cancellationToken);
            await _bookingRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Booking {BookingId} updated successfully", id);

            return Ok(MapToDto(booking));
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain validation failed");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating booking");
            return BadRequest(new { message = "An error occurred while updating the booking" });
        }
    }

    /// <summary>
    /// DELETE /api/bookings/{id} - Cancel/Delete booking
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Deleting booking {BookingId}", id);

            var bookingId = BookingId.Create(id);
            var booking = await _bookingRepository.GetByIdAsync(bookingId, cancellationToken);

            if (booking == null)
            {
                _logger.LogWarning("Booking {BookingId} not found", id);
                return NotFound(new { message = $"Booking with ID {id} not found" });
            }

            // Cancel booking (domain method)
            booking.Cancel();

            // Publish cancellation event to Kafka
            var events = booking.GetDomainEvents();
            foreach (var domainEvent in events)
            {
                await _kafkaProducer.PublishAsync(domainEvent, cancellationToken);
            }
            booking.ClearDomainEvents();

            // Save changes
            await _bookingRepository.UpdateAsync(booking, cancellationToken);
            await _bookingRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Booking {BookingId} cancelled successfully", id);

            return NoContent();
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain validation failed");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting booking");
            return BadRequest(new { message = "An error occurred while deleting the booking" });
        }
    }

    /// <summary>
    /// Map Booking entity to DTO
    /// </summary>
    private static BookingDto MapToDto(Booking booking)
    {
        return new BookingDto
        {
            BookingId = booking.Id.Value,
            GuestId = booking.GuestId.Value,
            CampsiteId = booking.CampsiteId.Value,
            AccommodationTypeId = booking.AccommodationTypeId.Value,
            CheckInDate = booking.Period.StartDate,
            CheckOutDate = booking.Period.EndDate,
            Status = booking.Status.ToString(),
            BasePriceAmount = booking.BasePrice.Amount,
            TotalPriceAmount = booking.TotalPrice.Amount,
            Currency = booking.BasePrice.Currency,
            NumberOfAdults = booking.NumberOfAdults,
            NumberOfChildren = booking.NumberOfChildren,
            SpecialRequests = booking.SpecialRequests,
            CreatedDate = booking.CreatedDate
        };
    }
}

