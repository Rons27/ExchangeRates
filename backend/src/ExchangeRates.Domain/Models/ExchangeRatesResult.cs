using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using ExchangeRates.Domain.Entities;

namespace ExchangeRates.Domain.Models
{

/// <summary>
/// Aggregated result returned by the exchange rate data source.
/// </summary>
public sealed class ExchangeRatesResult
{
    /// <summary>Date for which the rates are valid.</summary>
    public DateTime Date { get; init; }

    /// <summary>The base currency all rates are quoted against (always CZK for CNB).</summary>
    public string BaseCurrency { get; init; } = "CZK";

    /// <summary>Collection of individual exchange rates.</summary>
    public IReadOnlyList<ExchangeRate> Rates { get; init; } = Array.Empty<ExchangeRate>();
}
}
