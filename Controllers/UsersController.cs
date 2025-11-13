using CampsiteBooking.Infrastructure.Kafka;
using CampsiteBooking.Infrastructure.Security;
using CampsiteBooking.Models;
using CampsiteBooking.Models.Common;
using CampsiteBooking.Models.Repositories;
using CampsiteBooking.Models.ValueObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampsiteBooking.Controllers;

/// <summary>
/// REST API Controller for User CRUD operations.
/// Implements REST maturity level 2 (HTTP verbs + status codes).
/// Protected with JWT authentication.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize] // Require JWT authentication for all endpoints
public class UsersController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IKafkaProducer _kafkaProducer;
    private readonly ILogger<UsersController> _logger;

    public UsersController(
        IUserRepository userRepository,
        IKafkaProducer kafkaProducer,
        ILogger<UsersController> logger)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _kafkaProducer = kafkaProducer ?? throw new ArgumentNullException(nameof(kafkaProducer));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// GET /api/users/{id} - Get user by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> GetById(int id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting user {UserId}", id);

        var userId = UserId.Create(id);
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);

        if (user == null)
        {
            _logger.LogWarning("User {UserId} not found", id);
            return NotFound(new { message = $"User with ID {id} not found" });
        }

        return Ok(MapToDto(user));
    }

    /// <summary>
    /// POST /api/users - Create new user (Register)
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserDto>> Create(
        [FromBody] CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Creating user with email {Email}", request.Email);

            // XSS Protection: Validate user input
            InputValidator.ValidateForXss(request.FirstName, "FirstName");
            InputValidator.ValidateForXss(request.LastName, "LastName");
            InputValidator.ValidateLength(request.FirstName, "FirstName", 100, 1);
            InputValidator.ValidateLength(request.LastName, "LastName", 100, 1);

            if (!string.IsNullOrWhiteSpace(request.Phone))
            {
                InputValidator.ValidateForXss(request.Phone, "Phone");
                InputValidator.ValidateLength(request.Phone, "Phone", 20);
            }

            if (!string.IsNullOrWhiteSpace(request.Country))
            {
                InputValidator.ValidateForXss(request.Country, "Country");
                InputValidator.ValidateLength(request.Country, "Country", 100);
            }

            // Check if email already exists
            var email = Email.Create(request.Email);
            var emailExists = await _userRepository.EmailExistsAsync(email, cancellationToken);

            if (emailExists)
            {
                _logger.LogWarning("Email {Email} already exists", request.Email);
                return BadRequest(new { message = "Email already exists" });
            }

            // Use domain factory method to create user
            var user = Models.User.Create(
                email,
                request.FirstName,
                request.LastName,
                request.Phone ?? string.Empty,
                request.Country ?? string.Empty);

            // Save to database
            await _userRepository.AddAsync(user, cancellationToken);
            await _userRepository.SaveChangesAsync(cancellationToken);

            // Publish domain events to Kafka
            var events = user.GetDomainEvents();
            foreach (var domainEvent in events)
            {
                await _kafkaProducer.PublishAsync(domainEvent, cancellationToken);
            }
            user.ClearDomainEvents();

            _logger.LogInformation("User {UserId} created successfully", user.Id.Value);

            return CreatedAtAction(
                nameof(GetById),
                new { id = user.Id.Value },
                MapToDto(user));
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain validation failed");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user");
            return BadRequest(new { message = "An error occurred while creating the user" });
        }
    }

    /// <summary>
    /// Map User entity to DTO
    /// </summary>
    private static UserDto MapToDto(User user)
    {
        return new UserDto
        {
            UserId = user.Id.Value,
            Email = user.Email.Value,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Phone = user.Phone,
            Country = user.Country,
            JoinedDate = user.JoinedDate,
            IsActive = user.IsActive
        };
    }
}

