using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using ExchangeRates.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace ExchangeRates.Api.Middleware
{

public sealed class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            // Client disconnected — no need to log at Error level or write a response
            _logger.LogInformation("Request cancelled by client: {Path}", context.Request.Path);
        }
        catch (ExchangeRateProviderException ex)
        {
            _logger.LogError(ex, "Provider error for {Path}", context.Request.Path);
            await WriteProblemAsync(context, StatusCodes.Status502BadGateway,
                "Upstream Provider Error",
                "Unable to retrieve exchange rates from the upstream data source.");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "External HTTP error for {Path}", context.Request.Path);
            await WriteProblemAsync(context, StatusCodes.Status502BadGateway,
                "External Service Unavailable",
                "The upstream exchange rate service is currently unavailable.");
        }
        catch (TaskCanceledException ex) when (!context.RequestAborted.IsCancellationRequested)
        {
            _logger.LogError(ex, "Request timed out: {Path}", context.Request.Path);
            await WriteProblemAsync(context, StatusCodes.Status504GatewayTimeout,
                "Gateway Timeout",
                "The upstream exchange rate service did not respond in time.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception for {Path}", context.Request.Path);
            await WriteProblemAsync(context, StatusCodes.Status500InternalServerError,
                "Internal Server Error",
                "An unexpected error occurred. Please try again later.");
        }
    }

    private static async Task WriteProblemAsync(
        HttpContext context,
        int statusCode,
        string title,
        string detail)
    {
        if (context.Response.HasStarted)
            return;

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        var problem = new ProblemDetails
        {
            Status   = statusCode,
            Title    = title,
            Detail   = detail,
            Instance = context.Request.Path
        };

        var payload = JsonSerializer.Serialize(problem);
        await context.Response.WriteAsync(payload);
    }
}
}
