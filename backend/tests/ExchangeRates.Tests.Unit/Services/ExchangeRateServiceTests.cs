using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using ExchangeRates.Application.DTOs;
using ExchangeRates.Application.Interfaces;
using ExchangeRates.Application.Queries;
using ExchangeRates.Application.Services;
using ExchangeRates.Domain.Entities;
using ExchangeRates.Domain.Models;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace ExchangeRates.Tests.Unit.Services
{

public sealed class ExchangeRateServiceTests
{
    private readonly Mock<IExchangeRateProvider> _providerMock;
    private readonly ExchangeRateService _sut;

    public ExchangeRateServiceTests()
    {
        _providerMock = new Mock<IExchangeRateProvider>(MockBehavior.Strict);
        _sut = new ExchangeRateService(_providerMock.Object, NullLogger<ExchangeRateService>.Instance);
    }

    // ── GetExchangeRatesAsync ─────────────────────────────────────────────────

    [Fact]
    public async Task GetExchangeRatesAsync_NoFilter_ReturnsAllRates()
    {
        // Arrange
        var date = new DateTime(2024, 1, 15);
        var providerResult = BuildResult(date, BuildRate("EUR", 1, 25.34m), BuildRate("USD", 1, 23.12m));
        _providerMock
            .Setup(p => p.GetExchangeRatesAsync(null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(providerResult);

        // Act
        var result = await _sut.GetExchangeRatesAsync(new GetExchangeRatesQuery(), CancellationToken.None);

        // Assert
        result.Rates.Should().HaveCount(2);
        result.BaseCurrency.Should().Be("CZK");
        result.Date.Should().Be("2024-01-15");
    }

    [Fact]
    public async Task GetExchangeRatesAsync_WithCurrencyFilter_ReturnsMatchingRateOnly()
    {
        // Arrange
        var date = new DateTime(2024, 1, 15);
        var providerResult = BuildResult(date, BuildRate("EUR", 1, 25.34m), BuildRate("USD", 1, 23.12m));
        _providerMock
            .Setup(p => p.GetExchangeRatesAsync(null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(providerResult);

        // Act
        var result = await _sut.GetExchangeRatesAsync(
            new GetExchangeRatesQuery(Currency: "EUR"), CancellationToken.None);

        // Assert
        result.Rates.Should().ContainSingle(r => r.CurrencyCode == "EUR");
    }

    [Fact]
    public async Task GetExchangeRatesAsync_CurrencyFilterCaseInsensitive_ReturnsMatch()
    {
        // Arrange
        var date = new DateTime(2024, 1, 15);
        var providerResult = BuildResult(date, BuildRate("EUR", 1, 25.34m));
        _providerMock
            .Setup(p => p.GetExchangeRatesAsync(null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(providerResult);

        // Act
        var result = await _sut.GetExchangeRatesAsync(
            new GetExchangeRatesQuery(Currency: "eur"), CancellationToken.None);

        // Assert
        result.Rates.Should().ContainSingle();
    }

    [Fact]
    public async Task GetExchangeRatesAsync_WithDate_PassesDateToProvider()
    {
        // Arrange
        var date = new DateTime(2024, 6, 1);
        var providerResult = BuildResult(date, BuildRate("EUR", 1, 25.50m));
        _providerMock
            .Setup(p => p.GetExchangeRatesAsync(date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(providerResult);

        // Act
        var result = await _sut.GetExchangeRatesAsync(
            new GetExchangeRatesQuery(Date: date), CancellationToken.None);

        // Assert
        result.Date.Should().Be("2024-06-01");
        _providerMock.Verify(p => p.GetExchangeRatesAsync(date, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetExchangeRatesAsync_RatesAreSortedByCurrencyCode()
    {
        // Arrange
        var date = new DateTime(2024, 1, 15);
        var providerResult = BuildResult(date, BuildRate("USD", 1, 23m), BuildRate("EUR", 1, 25m), BuildRate("CHF", 1, 26m));
        _providerMock
            .Setup(p => p.GetExchangeRatesAsync(null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(providerResult);

        // Act
        var result = await _sut.GetExchangeRatesAsync(new GetExchangeRatesQuery(), CancellationToken.None);

        // Assert
        result.Rates.Select(r => r.CurrencyCode)
            .Should().BeInAscendingOrder();
    }

    [Fact]
    public async Task GetExchangeRatesAsync_MapsAllDtoFieldsCorrectly()
    {
        // Arrange
        var date = new DateTime(2024, 1, 15);
        var rate  = BuildRate("EUR", 1, 25.34m, "euro", "Eurozone");
        var providerResult = BuildResult(date, rate);
        _providerMock
            .Setup(p => p.GetExchangeRatesAsync(null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(providerResult);

        // Act
        var result = await _sut.GetExchangeRatesAsync(new GetExchangeRatesQuery(), CancellationToken.None);

        // Assert
        var dto = result.Rates.Single();
        dto.Should().BeEquivalentTo(new ExchangeRateDto("EUR", "euro", "Eurozone", 1, 25.34m));
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static ExchangeRatesResult BuildResult(DateTime date, params ExchangeRate[] rates) =>
        new() { Date = date, BaseCurrency = "CZK", Rates = rates };

    private static ExchangeRate BuildRate(
        string code,
        int amount,
        decimal rate,
        string currency = "test",
        string country = "Test Country") =>
        new()
        {
            CurrencyCode = code,
            Currency     = currency,
            Country      = country,
            Amount       = amount,
            Rate         = rate,
            ValidFor     = DateTime.MinValue
        };
}
}
