using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ExchangeRates.Domain.Entities
{

/// <summary>
/// Represents a single currency exchange rate against the Czech Koruna (CZK).
/// </summary>
public sealed class ExchangeRate
{
    /// <summary>ISO 4217 currency code (e.g. "EUR", "USD").</summary>
    public string CurrencyCode { get; init; } = string.Empty;

    /// <summary>Human-readable currency name (e.g. "euro", "dollar").</summary>
    public string Currency { get; init; } = string.Empty;

    /// <summary>Country or region the currency belongs to.</summary>
    public string Country { get; init; } = string.Empty;

    /// <summary>Base amount used to express the rate (typically 1, sometimes 100).</summary>
    public int Amount { get; init; }

    /// <summary>Exchange rate for <see cref="Amount"/> units of the foreign currency in CZK.</summary>
    public decimal Rate { get; init; }

    /// <summary>The date for which this rate is valid (date part only).</summary>
    public DateTime ValidFor { get; init; }
}
}
