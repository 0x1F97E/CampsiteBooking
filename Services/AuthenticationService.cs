using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using CampsiteBooking.Data;
using CampsiteBooking.Models.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace CampsiteBooking.Services;

/// <summary>
/// Custom authentication service for Blazor Server with cookie-based authentication
/// </summary>
public class AuthenticationService
{
    private readonly IDbContextFactory<CampsiteBookingDbContext> _dbContextFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthenticationService(
        IDbContextFactory<CampsiteBookingDbContext> dbContextFactory,
        IHttpContextAccessor httpContextAccessor)
    {
        _dbContextFactory = dbContextFactory;
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Authenticate user with email and password
    /// </summary>
    public async Task<AuthenticationResult> LoginAsync(string email, string password, bool rememberMe)
    {
        try
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();

            // Find user by email (load all users first for client-side evaluation)
            var allUsers = await context.Users.ToListAsync();
            var user = allUsers.FirstOrDefault(u => u.Email.Value.Equals(email, StringComparison.OrdinalIgnoreCase));

            if (user == null)
            {
                return AuthenticationResult.Failure("Invalid email or password.");
            }

            // TODO: Verify password hash (currently accepting any password for demo)
            // In production, use BCrypt.Net-Next to verify password hash
            Console.WriteLine($"⚠️ WARNING: Password verification skipped for demo purposes!");

            // Update last login time
            user.UpdateLastLogin();
            await context.SaveChangesAsync();

            // Create claims for the user
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.Value.ToString()),
                new Claim(ClaimTypes.Email, user.Email.Value),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim("FirstName", user.FirstName),
                new Claim("LastName", user.LastName)
            };

            // Add role claim based on user type
            var userType = user.GetType().Name;
            claims.Add(new Claim(ClaimTypes.Role, userType));

            var identity = new ClaimsIdentity(claims, "CookieAuth");
            var principal = new ClaimsPrincipal(identity);

            // Sign in the user with cookie authentication
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                var authProperties = new Microsoft.AspNetCore.Authentication.AuthenticationProperties
                {
                    IsPersistent = rememberMe, // Remember Me functionality
                    ExpiresUtc = rememberMe 
                        ? DateTimeOffset.UtcNow.AddDays(30) // 30 days if "Remember Me" is checked
                        : DateTimeOffset.UtcNow.AddHours(8)  // 8 hours session if not checked
                };

                await httpContext.SignInAsync(
                    "CookieAuth",
                    principal,
                    authProperties);

                Console.WriteLine($"✅ User {user.Email.Value} logged in successfully (UserId: {user.Id.Value}, RememberMe: {rememberMe})");
            }

            return AuthenticationResult.Success(user.Id.Value, user.Email.Value, $"{user.FirstName} {user.LastName}", userType);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Login error: {ex.Message}");
            return AuthenticationResult.Failure("An error occurred during login. Please try again.");
        }
    }

    /// <summary>
    /// Log out the current user
    /// </summary>
    public async Task LogoutAsync()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext != null)
        {
            await httpContext.SignOutAsync("CookieAuth");
            Console.WriteLine("✅ User logged out successfully");
        }
    }

    /// <summary>
    /// Get the current authenticated user's ID
    /// </summary>
    public int? GetCurrentUserId()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var userIdClaim = httpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (int.TryParse(userIdClaim, out int userId))
        {
            return userId;
        }

        return null;
    }

    /// <summary>
    /// Check if the current user is authenticated
    /// </summary>
    public bool IsAuthenticated()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        return httpContext?.User?.Identity?.IsAuthenticated ?? false;
    }
}

/// <summary>
/// Result of authentication attempt
/// </summary>
public class AuthenticationResult
{
    public bool IsSuccess { get; set; }
    public string ErrorMessage { get; set; } = "";
    public int UserId { get; set; }
    public string Email { get; set; } = "";
    public string FullName { get; set; } = "";
    public string UserType { get; set; } = "";

    public static AuthenticationResult Success(int userId, string email, string fullName, string userType)
    {
        return new AuthenticationResult
        {
            IsSuccess = true,
            UserId = userId,
            Email = email,
            FullName = fullName,
            UserType = userType
        };
    }

    public static AuthenticationResult Failure(string errorMessage)
    {
        return new AuthenticationResult
        {
            IsSuccess = false,
            ErrorMessage = errorMessage
        };
    }
}

