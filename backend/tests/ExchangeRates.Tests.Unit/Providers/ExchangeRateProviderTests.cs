using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using ExchangeRates.Application.Interfaces;
using ExchangeRates.Domain.Exceptions;
using ExchangeRates.Infrastructure.HttpClients;
using ExchangeRates.Infrastructure.Configuration;
using ExchangeRates.Infrastructure.Providers;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Xunit;

namespace ExchangeRates.Tests.Unit.Providers
{

public sealed class ExchangeRateProviderTests
{
    // ── GetExchangeRatesAsync ─────────────────────────────────────────────────

    [Fact]
    public async Task GetExchangeRatesAsync_ValidResponse_ReturnsMappedDomainObject()
    {
        // Arrange
                const string json = @"{
    ""rates"": [
        {
            ""validFor"": ""2024-01-15"",
            ""order"": 1,
            ""country"": ""Eurozone"",
            ""currency"": ""euro"",
            ""amount"": 1,
            ""currencyCode"": ""EUR"",
            ""rate"": 25.34
        }
    ]
}";

        var client = BuildCnbApiClient(HttpStatusCode.OK, json);
        var sut = new ExchangeRateProvider(client, NullLogger<ExchangeRateProvider>.Instance);

        // Act
        var result = await sut.GetExchangeRatesAsync(null);

        // Assert
        result.BaseCurrency.Should().Be("CZK");
        result.Date.Should().Be(new DateTime(2024, 1, 15));
        result.Rates.Should().ContainSingle(r =>
            r.CurrencyCode == "EUR" &&
            r.Country      == "Eurozone" &&
            r.Rate         == 25.34m &&
            r.Amount       == 1);
    }

    [Fact]
    public async Task GetExchangeRatesAsync_EmptyRatesList_ReturnsEmptyResult()
    {
        // Arrange
        const string json = "{ \"rates\": [] }";
        var client = BuildCnbApiClient(HttpStatusCode.OK, json);
        var sut = new ExchangeRateProvider(client, NullLogger<ExchangeRateProvider>.Instance);

        // Act
        var result = await sut.GetExchangeRatesAsync(null);

        // Assert
        result.Rates.Should().BeEmpty();
    }

    [Fact]
    public async Task GetExchangeRatesAsync_HttpError_ThrowsProviderException()
    {
        // Arrange
        var client = BuildCnbApiClient(HttpStatusCode.ServiceUnavailable, string.Empty);
        var sut = new ExchangeRateProvider(client, NullLogger<ExchangeRateProvider>.Instance);

        // Act
        var act = () => sut.GetExchangeRatesAsync(null);

        // Assert
        await act.Should().ThrowAsync<ExchangeRateProviderException>()
            .WithMessage("*CNB API*");
    }

    [Fact]
    public async Task GetExchangeRatesAsync_WithDate_IncludesDateInRequest()
    {
        // Arrange
                var date = new DateTime(2024, 6, 1);
                const string json = @"{
    ""rates"": [
        {
            ""validFor"": ""2024-06-01"",
            ""order"": 1,
            ""country"": ""Eurozone"",
            ""currency"": ""euro"",
            ""amount"": 1,
            ""currencyCode"": ""EUR"",
            ""rate"": 25.50
        }
    ]
}";

        string? capturedUrl = null;
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>((req, _) =>
                capturedUrl = req.RequestUri?.ToString())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
            });

        var httpClient = new HttpClient(handlerMock.Object)
            { BaseAddress = new Uri("https://api.cnb.cz") };

        var options = Options.Create(new CnbApiOptions
        {
            BaseUrl        = "https://api.cnb.cz",
            DailyRatesPath = "/cnbapi/exrates/daily",
            TimeoutSeconds = 30,
            RetryCount     = 0
        });

        var apiClient = new CnbApiClient(httpClient, options, NullLogger<CnbApiClient>.Instance);
        var sut = new ExchangeRateProvider(apiClient, NullLogger<ExchangeRateProvider>.Instance);

        // Act
        await sut.GetExchangeRatesAsync(date);

        // Assert
        capturedUrl.Should().Contain("date=2024-06-01");
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static CnbApiClient BuildCnbApiClient(HttpStatusCode statusCode, string json)
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
            });

        var httpClient = new HttpClient(handlerMock.Object)
            { BaseAddress = new Uri("https://api.cnb.cz") };

        var options = Options.Create(new CnbApiOptions
        {
            BaseUrl        = "https://api.cnb.cz",
            DailyRatesPath = "/cnbapi/exrates/daily",
            TimeoutSeconds = 30,
            RetryCount     = 0
        });

        return new CnbApiClient(httpClient, options, NullLogger<CnbApiClient>.Instance);
    }
}
}
