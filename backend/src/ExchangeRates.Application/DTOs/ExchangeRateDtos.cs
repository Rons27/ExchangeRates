using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ExchangeRates.Application.DTOs
{

public sealed record ExchangeRateDto(
    string CurrencyCode,
    string Currency,
    string Country,
    int Amount,
    decimal Rate);
public sealed record ExchangeRatesResponseDto(
    string Date,
    string BaseCurrency,
    IReadOnlyList<ExchangeRateDto> Rates);
}
