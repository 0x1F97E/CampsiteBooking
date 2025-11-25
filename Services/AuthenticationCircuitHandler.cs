using Microsoft.AspNetCore.Components.Server.Circuits;
using System.Security.Claims;

namespace CampsiteBooking.Services;

/// <summary>
/// Circuit handler that captures authentication state from the initial HTTP request
/// and makes it available throughout the SignalR circuit lifecycle.
/// </summary>
public class AuthenticationCircuitHandler : CircuitHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<AuthenticationCircuitHandler> _logger;
    
    // Store the authentication state for this circuit
    public ClaimsPrincipal? User { get; private set; }

    public AuthenticationCircuitHandler(
        IHttpContextAccessor httpContextAccessor,
        ILogger<AuthenticationCircuitHandler> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public override Task OnConnectionUpAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        // Capture the user from HttpContext when the circuit is first established
        var httpContext = _httpContextAccessor.HttpContext;

        _logger.LogInformation($"🔌 Circuit {circuit.Id}: OnConnectionUpAsync called");
        _logger.LogInformation($"   HttpContext: {(httpContext != null ? "Available" : "NULL")}");

        if (httpContext != null)
        {
            _logger.LogInformation($"   HttpContext.User: {(httpContext.User != null ? "Available" : "NULL")}");
            _logger.LogInformation($"   HttpContext.User.Identity: {(httpContext.User?.Identity != null ? "Available" : "NULL")}");
            _logger.LogInformation($"   HttpContext.User.Identity.IsAuthenticated: {httpContext.User?.Identity?.IsAuthenticated}");
            _logger.LogInformation($"   HttpContext.User.Identity.Name: {httpContext.User?.Identity?.Name}");

            // Log all cookies
            if (httpContext.Request.Cookies.Any())
            {
                _logger.LogInformation($"   Cookies: {string.Join(", ", httpContext.Request.Cookies.Keys)}");
            }
            else
            {
                _logger.LogWarning($"   No cookies found in request");
            }
        }

        if (httpContext?.User?.Identity?.IsAuthenticated == true)
        {
            User = httpContext.User;
            _logger.LogInformation($"✅ Circuit {circuit.Id}: Captured authenticated user - {User.FindFirst(ClaimTypes.Name)?.Value}, Role: {User.FindFirst(ClaimTypes.Role)?.Value}");
        }
        else
        {
            User = new ClaimsPrincipal(new ClaimsIdentity());
            _logger.LogWarning($"⚠️ Circuit {circuit.Id}: No authenticated user");
        }

        return base.OnConnectionUpAsync(circuit, cancellationToken);
    }

    public override Task OnCircuitClosedAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"🔌 Circuit {circuit.Id}: Closed");
        return base.OnCircuitClosedAsync(circuit, cancellationToken);
    }
}

