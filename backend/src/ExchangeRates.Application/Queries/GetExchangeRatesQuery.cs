using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ExchangeRates.Application.Queries
{

/// <summary>
/// Encapsulates the optional filter criteria for the GetExchangeRates use-case.
/// </summary>
/// <param name="Date">
/// Optional date. When provided, historical rates for that day are returned.
/// Must be formatted as yyyy-MM-dd. If null, today's rates are used.
/// </param>
/// <param name="Currency">
/// Optional ISO 4217 currency code filter (e.g. "EUR", "USD").
/// When provided only the matching currency row is returned.
/// </param>
public sealed record GetExchangeRatesQuery(
    DateTime? Date = null,
    string? Currency = null);
}
