using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace CampsiteBooking.Services;

/// <summary>
/// Authentication state provider for Blazor Server that reads authentication state
/// from the AuthenticationCircuitHandler (which captures it from the initial HTTP request).
/// </summary>
public class ServerAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly AuthenticationCircuitHandler _circuitHandler;
    private readonly ILogger<ServerAuthenticationStateProvider> _logger;

    public ServerAuthenticationStateProvider(
        AuthenticationCircuitHandler circuitHandler,
        ILogger<ServerAuthenticationStateProvider> logger)
    {
        _circuitHandler = circuitHandler;
        _logger = logger;
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var user = _circuitHandler.User;

        if (user?.Identity?.IsAuthenticated == true)
        {
            _logger.LogInformation($"✅ ServerAuthStateProvider: User authenticated - {user.FindFirst(ClaimTypes.Name)?.Value}, Role: {user.FindFirst(ClaimTypes.Role)?.Value}");
            return Task.FromResult(new AuthenticationState(user));
        }

        _logger.LogWarning("⚠️ ServerAuthStateProvider: User not authenticated");
        return Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
    }

    /// <summary>
    /// Notify that the authentication state has changed (e.g., after login/logout)
    /// </summary>
    public void NotifyAuthenticationStateChanged()
    {
        _logger.LogInformation("🔄 Authentication state changed - notifying components");
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}

