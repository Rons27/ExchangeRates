using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using ExchangeRates.Application.DTOs;
using ExchangeRates.Application.Interfaces;
using ExchangeRates.Application.Queries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace ExchangeRates.Api.Controllers
{
[ApiController]
[Route("api/exchange-rates")]
[Produces("application/json")]
public sealed class ExchangeRatesController : ControllerBase
{
    private readonly IExchangeRateService _service;


    public ExchangeRatesController(
        IExchangeRateService service,
        ILogger<ExchangeRatesController> logger)
    {
        _service = service;

    }

    [HttpGet]
    [ProducesResponseType(typeof(ExchangeRatesResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status502BadGateway)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetExchangeRates([FromQuery] string? date, [FromQuery] string? currency,  CancellationToken cancellationToken)
    {

        DateTime? parsedDate = null;
        if (!string.IsNullOrWhiteSpace(date))
        {
            if (!DateTime.TryParseExact(
                    date.Trim(),
                    "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                    out var d))
            {
                return Problem(
                    detail: $"'{date}' is not a valid date. Expected format: yyyy-MM-dd.",
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Invalid Date Parameter");
            }

            if (d.Date > DateTime.UtcNow.Date)
            {
                return Problem(
                    detail: "Future dates are not supported. Provide today's date or a past date.",
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Invalid Date Parameter");
            }

            parsedDate = d.Date;
        }

        // Validate optional currency parameter (basic guard — full validation via the provider)
        if (!string.IsNullOrWhiteSpace(currency) && currency.Trim().Length != 3)
        {
            return Problem(
                detail: $"'{currency}' is not a valid ISO 4217 currency code. Currency codes are 3 characters long.",
                statusCode: StatusCodes.Status400BadRequest,
                title: "Invalid Currency Parameter");
        }

        var query = new GetExchangeRatesQuery(parsedDate, currency?.Trim().ToUpperInvariant());
        var response = await _service.GetExchangeRatesAsync(query, cancellationToken);

        return Ok(response);
    }
}
}
