using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ExchangeRates.Infrastructure.Configuration
{

/// <summary>
/// Strongly-typed options for the Czech National Bank REST API.
/// Bound from the "CnbApi" section of appsettings.json.
/// </summary>
public sealed class CnbApiOptions
{
    /// <summary>The configuration section key.</summary>
    public const string SectionName = "CnbApi";

    /// <summary>Base URL of the CNB API, e.g. "https://api.cnb.cz".</summary>
    public string BaseUrl { get; set; } = string.Empty;

    /// <summary>Relative path to the daily exchange rates endpoint.</summary>
    public string DailyRatesPath { get; set; } = "/cnbapi/exrates/daily";

    /// <summary>HTTP request timeout in seconds.</summary>
    public int TimeoutSeconds { get; set; } = 30;

    /// <summary>Number of Polly retry attempts on transient failures.</summary>
    public int RetryCount { get; set; } = 3;
}
}
