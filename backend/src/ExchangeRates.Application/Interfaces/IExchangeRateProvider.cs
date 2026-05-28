using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using System.Threading;
using System.Threading.Tasks;
using ExchangeRates.Domain.Models;

namespace ExchangeRates.Application.Interfaces
{

/// <summary>
/// Abstraction over the external exchange rate data source.
/// Implementations live in the Infrastructure layer.
/// </summary>
public interface IExchangeRateProvider
{
    /// <summary>
    /// Retrieves exchange rates for the given date.
    /// If <paramref name="date"/> is null, the latest available rates are returned.
    /// </summary>
    Task<ExchangeRatesResult> GetExchangeRatesAsync(
        DateTime? date,
        CancellationToken cancellationToken = default);
}
}
