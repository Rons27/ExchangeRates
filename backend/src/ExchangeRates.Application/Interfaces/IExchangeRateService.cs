using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using ExchangeRates.Application.DTOs;
using ExchangeRates.Application.Queries;

namespace ExchangeRates.Application.Interfaces
{

/// <summary>
/// Application-layer service that orchestrates exchange rate retrieval and filtering.
/// </summary>
public interface IExchangeRateService
{
    /// <summary>
    /// Returns exchange rates that satisfy the supplied query parameters.
    /// </summary>
    Task<ExchangeRatesResponseDto> GetExchangeRatesAsync(
        GetExchangeRatesQuery query,
        CancellationToken cancellationToken = default);
}
}
