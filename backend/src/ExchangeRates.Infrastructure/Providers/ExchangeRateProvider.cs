using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ExchangeRates.Application.Interfaces;
using ExchangeRates.Domain.Entities;
using ExchangeRates.Domain.Exceptions;
using ExchangeRates.Domain.Models;
using ExchangeRates.Infrastructure.HttpClients;
using Microsoft.Extensions.Logging;

namespace ExchangeRates.Infrastructure.Providers
{

/// <summary>
/// Implements <see cref="IExchangeRateProvider"/> by delegating to the CNB API client
/// and mapping the raw HTTP response into domain objects.
/// </summary>
public sealed class ExchangeRateProvider : IExchangeRateProvider
{
    private readonly CnbApiClient _apiClient;
    private readonly ILogger<ExchangeRateProvider> _logger;

    public ExchangeRateProvider(
        CnbApiClient apiClient,
        ILogger<ExchangeRateProvider> logger)
    {
        _apiClient = apiClient;
    }

    public async Task<ExchangeRatesResult> GetExchangeRatesAsync(
        DateTime? date,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _apiClient.GetDailyRatesAsync(date, cancellationToken);

            if (response.Rates.Count == 0)
            {
                _logger.LogWarning(
                    "CNB API returned zero rates for date '{Date}'",
                    date?.ToString("yyyy-MM-dd") ?? "today");

                return new ExchangeRatesResult
                {
                    Date = (date ?? DateTime.UtcNow).Date,
                    BaseCurrency = "CZK",
                    Rates = Array.Empty<ExchangeRate>()
                };
            }

            if (!DateTime.TryParse(response.Rates[0].ValidFor, out var validFor))
            {
                throw new ExchangeRateProviderException(
                    $"CNB API returned an unparseable date: '{response.Rates[0].ValidFor}'");
            }

            validFor = validFor.Date;

            var rates = response.Rates.Select(item => new ExchangeRate
            {
                CurrencyCode = item.CurrencyCode,
                Currency     = item.Currency,
                Country      = item.Country,
                Amount       = item.Amount,
                Rate         = item.Rate,
                ValidFor     = validFor
            }).ToList();

            return new ExchangeRatesResult
            {
                Date         = validFor,
                BaseCurrency = "CZK",
                Rates        = rates
            };
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error while fetching rates from CNB API");
            throw new ExchangeRateProviderException(
                "Failed to retrieve exchange rates from the CNB API. See inner exception for details.",
                ex);
        }
    }
}
}
