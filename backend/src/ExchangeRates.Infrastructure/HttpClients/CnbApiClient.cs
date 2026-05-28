using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeRates.Infrastructure.Configuration;
using ExchangeRates.Infrastructure.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ExchangeRates.Infrastructure.HttpClients
{

/// <summary>
/// Typed HTTP client for the Czech National Bank (CNB) REST API.
/// Responsible only for the raw HTTP communication; it does not map to domain objects.
/// </summary>
public sealed class CnbApiClient
{
    private readonly HttpClient _httpClient;
    private readonly CnbApiOptions _options;
    private readonly ILogger<CnbApiClient> _logger;

    public CnbApiClient(
        HttpClient httpClient,
        IOptions<CnbApiOptions> options,
        ILogger<CnbApiClient> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    /// <summary>
    /// Fetches the daily exchange rate list from the CNB API.
    /// </summary>
    /// <param name="date">
    /// Optional date. When null the CNB API returns the most recent available rates.
    /// </param>
    /// <param name="cancellationToken">Propagated cancellation token.</param>
    public async Task<CnbDailyRatesResponse> GetDailyRatesAsync(
        DateTime? date,
        CancellationToken cancellationToken = default)
    {
        var path = BuildRequestPath(date);

        _logger.LogInformation("GET {Path}", path);

        var response = await _httpClient.GetAsync(path, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(); 
            _logger.LogWarning(
                "CNB API returned {StatusCode}. Body: {Body}",
                (int)response.StatusCode,
                body);
            response.EnsureSuccessStatusCode(); // throws HttpRequestException
        }

        var result = await response.Content.ReadFromJsonAsync<CnbDailyRatesResponse>(
            cancellationToken: cancellationToken);

        return result ?? new CnbDailyRatesResponse();
    }

    // ──────────────────────────────────────────────────────────────────────────

    private string BuildRequestPath(DateTime? date)
    {
        var path = _options.DailyRatesPath;
        var query = date.HasValue
            ? $"?date={date.Value:yyyy-MM-dd}&lang=EN"
            : "?lang=EN";
        return path + query;
    }
}
}
