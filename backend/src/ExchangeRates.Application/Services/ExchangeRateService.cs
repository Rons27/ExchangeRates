using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using ExchangeRates.Application.DTOs;
using ExchangeRates.Application.Interfaces;
using ExchangeRates.Application.Queries;
using Microsoft.Extensions.Logging;

namespace ExchangeRates.Application.Services
{

/// <summary>
/// Orchestrates exchange rate retrieval from the provider and applies optional filtering.
/// This is a pure application-layer service: it contains no I/O, only coordination logic.
/// </summary>
public sealed class ExchangeRateService : IExchangeRateService
{
    private readonly IExchangeRateProvider _provider;
    private readonly ILogger<ExchangeRateService> _logger;

    public ExchangeRateService(
        IExchangeRateProvider provider,
        ILogger<ExchangeRateService> logger)
    {
        _provider = provider;
        _logger = logger;
    }

    public async Task<ExchangeRatesResponseDto> GetExchangeRatesAsync(
        GetExchangeRatesQuery query,
        CancellationToken cancellationToken = default)
    {
        var result = await _provider.GetExchangeRatesAsync(query.Date, cancellationToken);

        var rates = result.Rates.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(query.Currency))
        {
            rates = rates.Where(r =>
                r.CurrencyCode.Equals(query.Currency.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        var rateDtos = rates
            .OrderBy(r => r.CurrencyCode)
            .Select(r => new ExchangeRateDto(
                r.CurrencyCode,
                r.Currency,
                r.Country,
                r.Amount,
                r.Rate))
            .ToList();

     
        return new ExchangeRatesResponseDto(
            result.Date.ToString("yyyy-MM-dd"),
            result.BaseCurrency,
            rateDtos);
    }
}
}
