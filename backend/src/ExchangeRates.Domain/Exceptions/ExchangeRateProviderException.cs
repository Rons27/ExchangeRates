using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ExchangeRates.Domain.Exceptions
{

/// <summary>
/// Thrown when the upstream exchange rate provider returns an unexpected or invalid response.
/// </summary>
public sealed class ExchangeRateProviderException : Exception
{
    public ExchangeRateProviderException(string message)
        : base(message) { }

    public ExchangeRateProviderException(string message, Exception innerException)
        : base(message, innerException) { }
}
}
