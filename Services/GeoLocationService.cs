using System.Net.Http;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace CampsiteBooking.Services;

public class GeoLocationService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public GeoLocationService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
    {
        _httpClientFactory = httpClientFactory;
        _httpContextAccessor = httpContextAccessor;
    }
    
    public async Task<string> GetLanguageFromIpAsync()
    {
        try
        {
            var ipAddress = GetClientIpAddress();
            
            // For localhost/development, return English as default
            if (string.IsNullOrEmpty(ipAddress) || 
                ipAddress == "::1" || 
                ipAddress == "127.0.0.1" || 
                ipAddress.StartsWith("192.168.") ||
                ipAddress.StartsWith("10.") ||
                ipAddress.StartsWith("172."))
            {
                return "en"; // Default to English for local/private IPs
            }
            
            // Use ip-api.com (free, no API key required, 45 requests/minute)
            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync($"http://ip-api.com/json/{ipAddress}?fields=countryCode");
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var geoData = JsonSerializer.Deserialize<GeoLocationResponse>(json);
                
                if (geoData?.CountryCode != null)
                {
                    return MapCountryCodeToLanguage(geoData.CountryCode);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting geolocation: {ex.Message}");
        }
        
        return "en"; // Default to English if geolocation fails
    }
    
    private string GetClientIpAddress()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null) return string.Empty;
        
        // Check for forwarded IP (when behind proxy/load balancer)
        var forwardedFor = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
        {
            return forwardedFor.Split(',')[0].Trim();
        }
        
        // Check for real IP header
        var realIp = httpContext.Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(realIp))
        {
            return realIp;
        }
        
        // Fall back to remote IP address
        return httpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
    }
    
    private string MapCountryCodeToLanguage(string countryCode)
    {
        return countryCode.ToUpper() switch
        {
            "DK" => "da",      // Denmark -> Danish
            "DE" => "de",      // Germany -> German
            "AT" => "de",      // Austria -> German
            "CH" => "de",      // Switzerland -> German (default)
            "SE" => "sv",      // Sweden -> Swedish
            "NO" => "no",      // Norway -> Norwegian
            "NL" => "nl",      // Netherlands -> Dutch
            "BE" => "nl",      // Belgium -> Dutch (default)
            "GB" => "en-gb",   // United Kingdom -> English (UK)
            "IE" => "en-gb",   // Ireland -> English (UK)
            "US" => "en",      // United States -> English
            "CA" => "en",      // Canada -> English (default)
            "AU" => "en-gb",   // Australia -> English (UK)
            "NZ" => "en-gb",   // New Zealand -> English (UK)
            _ => "en"          // Default to English for all other countries
        };
    }
}

public class GeoLocationResponse
{
    public string? CountryCode { get; set; }
}

