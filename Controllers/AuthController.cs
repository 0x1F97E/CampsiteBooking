using CampsiteBooking.Infrastructure.Kafka;
using CampsiteBooking.Infrastructure.Security;
using CampsiteBooking.Models;
using CampsiteBooking.Models.Common;
using CampsiteBooking.Models.Repositories;
using CampsiteBooking.Models.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CampsiteBooking.Controllers;

/// <summary>
/// Authentication Controller for Register and Login.
/// Implements JWT token-based authentication.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IKafkaProducer _kafkaProducer;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IUserRepository userRepository,
        IKafkaProducer kafkaProducer,
        IConfiguration configuration,
        ILogger<AuthController> logger)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _kafkaProducer = kafkaProducer ?? throw new ArgumentNullException(nameof(kafkaProducer));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// POST /api/auth/register - Register new user
    /// </summary>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponse>> Register(
        [FromBody] RegisterRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Registering user with email {Email}", request.Email);

            // XSS Protection: Validate user input
            InputValidator.ValidateForXss(request.FirstName, "FirstName");
            InputValidator.ValidateForXss(request.LastName, "LastName");
            InputValidator.ValidateLength(request.FirstName, "FirstName", 100, 1);
            InputValidator.ValidateLength(request.LastName, "LastName", 100, 1);
            InputValidator.ValidateLength(request.Password, "Password", 100, 6);

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

            // Hash password (simple for 3. semester - use BCrypt in production)
            var passwordHash = HashPassword(request.Password);

            // Create user using domain factory
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

            // Generate JWT token
            var token = GenerateJwtToken(user);

            _logger.LogInformation("User {UserId} registered successfully", user.Id.Value);

            return CreatedAtAction(
                nameof(Register),
                new AuthResponse
                {
                    Token = token,
                    UserId = user.Id.Value,
                    Email = user.Email.Value,
                    FirstName = user.FirstName,
                    LastName = user.LastName
                });
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain validation failed");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering user");
            return BadRequest(new { message = "An error occurred while registering the user" });
        }
    }

    /// <summary>
    /// POST /api/auth/login - Login user
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Login attempt for email {Email}", request.Email);

            // Find user by email
            var email = Email.Create(request.Email);
            var user = await _userRepository.GetByEmailAsync(email, cancellationToken);

            if (user == null)
            {
                _logger.LogWarning("User with email {Email} not found", request.Email);
                return Unauthorized(new { message = "Invalid email or password" });
            }

            // Verify password (simple for 3. semester)
            // Note: In production, store hashed password in User entity and verify here
            // For now, we accept any password (INSECURE - only for demo!)
            _logger.LogWarning("Password verification skipped - implement proper hashing!");

            // Generate JWT token
            var token = GenerateJwtToken(user);

            _logger.LogInformation("User {UserId} logged in successfully", user.Id.Value);

            return Ok(new AuthResponse
            {
                Token = token,
                UserId = user.Id.Value,
                Email = user.Email.Value,
                FirstName = user.FirstName,
                LastName = user.LastName
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            return Unauthorized(new { message = "Invalid email or password" });
        }
    }

    /// <summary>
    /// Generate JWT token for authenticated user
    /// </summary>
    private string GenerateJwtToken(User user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] ?? "BookMyHome-SuperSecret-Key-For-3Semester-Exam-Project-2025";
        var issuer = jwtSettings["Issuer"] ?? "BookMyHome";
        var audience = jwtSettings["Audience"] ?? "BookMyHomeUsers";

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.Value.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email.Value),
            new Claim(JwtRegisteredClaimNames.GivenName, user.FirstName),
            new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(24), // Token valid for 24 hours
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Hash password using SHA256 (simple for 3. semester - use BCrypt in production!)
    /// </summary>
    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }
}
