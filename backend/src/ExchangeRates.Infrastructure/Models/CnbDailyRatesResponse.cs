using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using System.Text.Json.Serialization;

namespace ExchangeRates.Infrastructure.Models
{

/// <summary>
/// Root response object returned by the CNB /exrates/daily JSON endpoint.
/// </summary>
public sealed class CnbDailyRatesResponse
{
    [JsonPropertyName("rates")]
    public List<CnbExchangeRateItem> Rates { get; set; } = new();
}

/// <summary>
/// A single currency entry in the CNB daily rates response.
/// </summary>
public sealed class CnbExchangeRateItem
{
    /// <summary>ISO date string for which the rate is valid (yyyy-MM-dd).</summary>
    [JsonPropertyName("validFor")]
    public string ValidFor { get; set; } = string.Empty;

    /// <summary>Publication order number within the release.</summary>
    [JsonPropertyName("order")]
    public int Order { get; set; }

    /// <summary>Country or region name.</summary>
    [JsonPropertyName("country")]
    public string Country { get; set; } = string.Empty;

    /// <summary>Human-readable currency name.</summary>
    [JsonPropertyName("currency")]
    public string Currency { get; set; } = string.Empty;

    /// <summary>Unit amount to which the rate applies.</summary>
    [JsonPropertyName("amount")]
    public int Amount { get; set; }

    /// <summary>ISO 4217 currency code.</summary>
    [JsonPropertyName("currencyCode")]
    public string CurrencyCode { get; set; } = string.Empty;

    /// <summary>Exchange rate for <see cref="Amount"/> units in CZK.</summary>
    [JsonPropertyName("rate")]
    public decimal Rate { get; set; }
}
}
