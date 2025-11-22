using System.Globalization;
using Microsoft.JSInterop;

namespace CampsiteBooking.Services;

public class LocalizationService
{
    private readonly IJSRuntime _jsRuntime;
    private CultureInfo _currentCulture;
    
    public event Action? OnCultureChanged;
    
    public LocalizationService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
        _currentCulture = CultureInfo.CurrentCulture;
    }
    
    public CultureInfo CurrentCulture => _currentCulture;
    
    public async Task InitializeAsync()
    {
        try
        {
            // Try to get stored language preference from localStorage
            var storedLanguage = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "preferredLanguage");
            
            if (!string.IsNullOrEmpty(storedLanguage))
            {
                await SetCultureAsync(storedLanguage);
            }
        }
        catch
        {
            // If localStorage is not available (e.g., during prerendering), use default
        }
    }
    
    public async Task SetCultureAsync(string cultureName)
    {
        try
        {
            var culture = new CultureInfo(cultureName);
            _currentCulture = culture;
            
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;
            
            // Store preference in localStorage
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "preferredLanguage", cultureName);
            
            OnCultureChanged?.Invoke();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error setting culture: {ex.Message}");
        }
    }
    
    public string GetLanguageCode() => _currentCulture.TwoLetterISOLanguageName;
    
    public string GetLanguageName() => _currentCulture.NativeName;
    
    public static Dictionary<string, LanguageInfo> SupportedLanguages => new()
    {
        { "da", new LanguageInfo("da-DK", "Dansk", "Denmark", "🇩🇰", "DK") },
        { "de", new LanguageInfo("de-DE", "Deutsch", "Germany", "🇩🇪", "DE") },
        { "sv", new LanguageInfo("sv-SE", "Svenska", "Sweden", "🇸🇪", "SE") },
        { "no", new LanguageInfo("nb-NO", "Norsk", "Norway", "🇳🇴", "NO") },
        { "nl", new LanguageInfo("nl-NL", "Nederlands", "Netherlands", "🇳🇱", "NL") },
        { "en-gb", new LanguageInfo("en-GB", "English", "United Kingdom", "🇬🇧", "GB") },
        { "en", new LanguageInfo("en-US", "English", "English", "🇬🇧", "US") }
    };
}

public record LanguageInfo(
    string CultureCode,
    string NativeName,
    string RegionName,
    string Flag,
    string CountryCode
);

