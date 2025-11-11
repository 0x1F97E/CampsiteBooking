using CampsiteBooking.Models.Common;
using CampsiteBooking.Models.ValueObjects;
using CampsiteBooking.Models.DomainEvents;

namespace CampsiteBooking.Models;

/// <summary>
/// Booking aggregate root representing a campsite reservation.
/// Enforces all booking business rules and invariants.
/// </summary>
public class Booking : AggregateRoot<BookingId>
{
    // ============================================================================
    // PRIVATE FIELDS - Encapsulation
    // ============================================================================

    private GuestId _guestId = null!;
    private CampsiteId _campsiteId = null!;
    private AccommodationSpotId? _accommodationSpotId;
    private AccommodationTypeId _accommodationTypeId = null!;
    private DateRange _period = null!;
    private BookingStatus _status = BookingStatus.Pending;
    private Money _basePrice = null!;
    private Money _totalPrice = null!;
    private int _numberOfAdults;
    private int _numberOfChildren;
    private string _specialRequests = string.Empty;
    private DateTime _createdDate;
    private DateTime _lastModifiedDate;
    private DateTime? _cancellationDate;

    // ============================================================================
    // PUBLIC PROPERTIES - Read-only access
    // ============================================================================

    /// <summary>Legacy property for EF Core and backward compatibility</summary>
    public int BookingId
    {
        get => Id?.Value ?? 0;
        private set => Id = value > 0 ? ValueObjects.BookingId.Create(value) : ValueObjects.BookingId.CreateNew();
    }

    public GuestId GuestId => _guestId;
    public CampsiteId CampsiteId => _campsiteId;
    public AccommodationSpotId? AccommodationSpotId => _accommodationSpotId;
    public AccommodationTypeId AccommodationTypeId => _accommodationTypeId;
    public DateRange Period => _period;
    public BookingStatus Status => _status;
    public Money BasePrice => _basePrice;
    public Money TotalPrice => _totalPrice;
    public int NumberOfAdults => _numberOfAdults;
    public int NumberOfChildren => _numberOfChildren;
    public string SpecialRequests => _specialRequests;
    public DateTime CreatedDate => _createdDate;
    public DateTime LastModifiedDate => _lastModifiedDate;
    public DateTime? CancellationDate => _cancellationDate;

    // Legacy properties for backward compatibility
    public int UserId => _guestId?.Value ?? 0;
    public DateTime CheckInDate => _period?.StartDate ?? DateTime.UtcNow.Date;
    public DateTime CheckOutDate => _period?.EndDate ?? DateTime.UtcNow.Date.AddDays(1);
    public string SpotId => _accommodationSpotId?.Value.ToString() ?? string.Empty;

    // ============================================================================
    // FACTORY METHODS - Controlled creation
    // ============================================================================

    /// <summary>
    /// Create a new booking (factory method)
    /// </summary>
    public static Booking Create(
        GuestId guestId,
        CampsiteId campsiteId,
        AccommodationTypeId accommodationTypeId,
        DateRange period,
        Money basePrice,
        int numberOfAdults,
        int numberOfChildren = 0,
        string specialRequests = "")
    {
        // Validate business rules
        if (period.StartDate < DateTime.UtcNow.Date)
            throw new DomainException("Cannot create booking with check-in date in the past");

        if (numberOfAdults <= 0)
            throw new DomainException("Booking must have at least one adult");

        if (numberOfAdults > 10)
            throw new DomainException("Maximum 10 adults per booking");

        if (numberOfChildren < 0)
            throw new DomainException("Number of children cannot be negative");

        if (numberOfChildren > 10)
            throw new DomainException("Maximum 10 children per booking");

        var booking = new Booking
        {
            Id = ValueObjects.BookingId.CreateNew(),
            _guestId = guestId,
            _campsiteId = campsiteId,
            _accommodationTypeId = accommodationTypeId,
            _period = period,
            _status = BookingStatus.Pending,
            _basePrice = basePrice,
            _totalPrice = basePrice, // Initially same as base price
            _numberOfAdults = numberOfAdults,
            _numberOfChildren = numberOfChildren,
            _specialRequests = specialRequests ?? string.Empty,
            _createdDate = DateTime.UtcNow,
            _lastModifiedDate = DateTime.UtcNow
        };

        // Raise domain event
        booking.RaiseDomainEvent(new BookingCreatedEvent(
            booking.Id.Value,
            guestId.Value,
            campsiteId.Value,
            period.StartDate,
            period.EndDate
        ));

        return booking;
    }

    // ============================================================================
    // PRIVATE CONSTRUCTOR - Prevent direct instantiation
    // ============================================================================

    private Booking()
    {
    }

    // ============================================================================
    // BUSINESS METHODS - Rich behavior
    // ============================================================================

    /// <summary>
    /// Assign an accommodation spot to this booking
    /// </summary>
    public void AssignAccommodationSpot(AccommodationSpotId spotId)
    {
        if (_status != BookingStatus.Pending)
            throw new DomainException("Can only assign spot to pending bookings");

        _accommodationSpotId = spotId;
        _lastModifiedDate = DateTime.UtcNow;
    }

    /// <summary>
    /// Confirm the booking
    /// </summary>
    public void Confirm()
    {
        if (!_status.CanTransitionTo(BookingStatus.Confirmed))
            throw new DomainException($"Cannot confirm booking in {_status} status");

        if (_accommodationSpotId == null)
            throw new DomainException("Cannot confirm booking without assigned accommodation spot");

        _status = BookingStatus.Confirmed;
        _lastModifiedDate = DateTime.UtcNow;

        RaiseDomainEvent(new BookingConfirmedEvent(Id.Value, _guestId.Value, _period.StartDate, _period.EndDate));
    }

    /// <summary>
    /// Cancel the booking
    /// </summary>
    public void Cancel()
    {
        if (!_status.CanTransitionTo(BookingStatus.Cancelled))
            throw new DomainException($"Cannot cancel booking in {_status} status");

        _status = BookingStatus.Cancelled;
        _cancellationDate = DateTime.UtcNow;
        _lastModifiedDate = DateTime.UtcNow;

        RaiseDomainEvent(new BookingCancelledEvent(Id.Value, _guestId.Value));
    }

    /// <summary>
    /// Complete the booking (guest has checked out)
    /// </summary>
    public void Complete()
    {
        if (!_status.CanTransitionTo(BookingStatus.Completed))
            throw new DomainException($"Cannot complete booking in {_status} status");

        _status = BookingStatus.Completed;
        _lastModifiedDate = DateTime.UtcNow;
    }

    /// <summary>
    /// Update the total price (e.g., after applying discounts or adding extras)
    /// </summary>
    public void UpdateTotalPrice(Money newTotalPrice)
    {
        if (_status == BookingStatus.Cancelled || _status == BookingStatus.Completed)
            throw new DomainException("Cannot update price of cancelled or completed booking");

        if (newTotalPrice.Amount < 0)
            throw new DomainException("Total price cannot be negative");

        _totalPrice = newTotalPrice;
        _lastModifiedDate = DateTime.UtcNow;
    }

    /// <summary>
    /// Update special requests
    /// </summary>
    public void UpdateSpecialRequests(string specialRequests)
    {
        if (_status == BookingStatus.Cancelled || _status == BookingStatus.Completed)
            throw new DomainException("Cannot update special requests of cancelled or completed booking");

        _specialRequests = specialRequests ?? string.Empty;
        _lastModifiedDate = DateTime.UtcNow;
    }

    /// <summary>
    /// Check if booking is active (not cancelled or completed)
    /// </summary>
    public bool IsActive()
    {
        return _status != BookingStatus.Cancelled && _status != BookingStatus.Completed;
    }

    /// <summary>
    /// Check if booking can be modified
    /// </summary>
    public bool CanBeModified()
    {
        return _status == BookingStatus.Pending || _status == BookingStatus.Confirmed;
    }

    /// <summary>
    /// Calculate number of nights
    /// </summary>
    public int GetNumberOfNights()
    {
        return _period.GetNumberOfDays();
    }

    /// <summary>
    /// Get total number of guests
    /// </summary>
    public int GetTotalGuests()
    {
        return _numberOfAdults + _numberOfChildren;
    }
}
