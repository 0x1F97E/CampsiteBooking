using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace CampsiteBooking.Services;

/// <summary>
/// Service for calling BookMyHome REST API from Blazor components.
/// </summary>
public class ApiService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ApiService> _logger;
    private string? _jwtToken;

    public ApiService(IHttpClientFactory httpClientFactory, ILogger<ApiService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public void SetAuthToken(string token) => _jwtToken = token;
    public void ClearAuthToken() => _jwtToken = null;

    public async Task<T?> GetAsync<T>(string endpoint, CancellationToken ct = default)
    {
        var client = CreateClient();
        var response = await client.GetAsync(endpoint, ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>(cancellationToken: ct);
    }

    public async Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest data, CancellationToken ct = default)
    {
        var client = CreateClient();
        var response = await client.PostAsJsonAsync(endpoint, data, ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken: ct);
    }

    public async Task<bool> DeleteAsync(string endpoint, CancellationToken ct = default)
    {
        var client = CreateClient();
        var response = await client.DeleteAsync(endpoint, ct);
        return response.IsSuccessStatusCode;
    }

    private HttpClient CreateClient()
    {
        var client = _httpClientFactory.CreateClient("BookMyHomeAPI");
        if (!string.IsNullOrWhiteSpace(_jwtToken))
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwtToken);
        return client;
    }
}

