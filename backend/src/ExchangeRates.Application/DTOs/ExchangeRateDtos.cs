using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ExchangeRates.Application.DTOs
{

/// <summary>Single exchange rate row returned in the API response.</summary>
/// <param name="CurrencyCode">ISO 4217 code, e.g. "EUR".</param>
/// <param name="Currency">Human-readable name, e.g. "euro".</param>
/// <param name="Country">Country / region, e.g. "Eurozone".</param>
/// <param name="Amount">The base unit amount the rate applies to (typically 1).</param>
/// <param name="Rate">Rate in CZK for <paramref name="Amount"/> units.</param>
public sealed record ExchangeRateDto(
    string CurrencyCode,
    string Currency,
    string Country,
    int Amount,
    decimal Rate);

/// <summary>Top-level response envelope returned by GET /api/exchange-rates.</summary>
/// <param name="Date">ISO date string (yyyy-MM-dd) for which the rates are valid.</param>
/// <param name="BaseCurrency">The base currency (always "CZK").</param>
/// <param name="Rates">List of individual exchange rate rows.</param>
public sealed record ExchangeRatesResponseDto(
    string Date,
    string BaseCurrency,
    IReadOnlyList<ExchangeRateDto> Rates);
}
