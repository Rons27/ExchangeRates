using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using ExchangeRates.Application.Interfaces;
using ExchangeRates.Application.Queries;
using ExchangeRates.Application.Services;
using ExchangeRates.Domain.Entities;
using ExchangeRates.Domain.Models;
using ExchangeRates.Api.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace ExchangeRates.Tests.Unit.Controllers
{

public sealed class ExchangeRatesControllerTests
{
    private readonly Mock<IExchangeRateService> _serviceMock;
    private readonly ExchangeRatesController _sut;

    public ExchangeRatesControllerTests()
    {
        _serviceMock = new Mock<IExchangeRateService>(MockBehavior.Strict);
        _sut = new ExchangeRatesController(
            _serviceMock.Object,
            NullLogger<ExchangeRatesController>.Instance);
    }

    [Fact]
    public async Task GetExchangeRates_NoParameters_Returns200WithRates()
    {
        // Arrange
        var dto = new Application.DTOs.ExchangeRatesResponseDto(
            "2024-01-15",
            "CZK",
            new[] { new Application.DTOs.ExchangeRateDto("EUR", "euro", "Eurozone", 1, 25.34m) });

        _serviceMock
            .Setup(s => s.GetExchangeRatesAsync(It.IsAny<GetExchangeRatesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(dto);

        // Act
        var result = await _sut.GetExchangeRates(null, null, CancellationToken.None);

        // Assert
        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        ok.StatusCode.Should().Be(200);
        ok.Value.Should().Be(dto);
    }

    [Theory]
    [InlineData("not-a-date")]
    [InlineData("15-01-2024")]
    [InlineData("2024/01/15")]
    [InlineData("20240115")]
    public async Task GetExchangeRates_InvalidDateFormat_Returns400(string badDate)
    {
        // Act
        var result = await _sut.GetExchangeRates(badDate, null, CancellationToken.None);

        // Assert
        var problem = result.Should().BeOfType<ObjectResult>().Subject;
        problem.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetExchangeRates_FutureDate_Returns400()
    {
        var futureDate = DateTime.UtcNow.AddDays(10).ToString("yyyy-MM-dd");

        var result = await _sut.GetExchangeRates(futureDate, null, CancellationToken.None);

        var problem = result.Should().BeOfType<ObjectResult>().Subject;
        problem.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }

    [Theory]
    [InlineData("EU")]      // too short
    [InlineData("EURO")]    // too long
    [InlineData("E")]       // single char
    public async Task GetExchangeRates_InvalidCurrencyCode_Returns400(string badCurrency)
    {
        var result = await _sut.GetExchangeRates(null, badCurrency, CancellationToken.None);

        var problem = result.Should().BeOfType<ObjectResult>().Subject;
        problem.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetExchangeRates_ValidDate_PassesDateToService()
    {
        // Arrange
        var dto = new Application.DTOs.ExchangeRatesResponseDto("2024-06-01", "CZK", Array.Empty<Application.DTOs.ExchangeRateDto>());
        GetExchangeRatesQuery? capturedQuery = null;

        _serviceMock
            .Setup(s => s.GetExchangeRatesAsync(It.IsAny<GetExchangeRatesQuery>(), It.IsAny<CancellationToken>()))
            .Callback<GetExchangeRatesQuery, CancellationToken>((q, _) => capturedQuery = q)
            .ReturnsAsync(dto);

        // Act
        await _sut.GetExchangeRates("2024-06-01", null, CancellationToken.None);

        // Assert
        capturedQuery.Should().NotBeNull();
        capturedQuery!.Date.Should().Be(new DateTime(2024, 6, 1));
    }

    [Fact]
    public async Task GetExchangeRates_CurrencyParam_IsNormalisedToUppercase()
    {
        // Arrange
        var dto = new Application.DTOs.ExchangeRatesResponseDto("2024-01-15", "CZK", Array.Empty<Application.DTOs.ExchangeRateDto>());
        GetExchangeRatesQuery? capturedQuery = null;

        _serviceMock
            .Setup(s => s.GetExchangeRatesAsync(It.IsAny<GetExchangeRatesQuery>(), It.IsAny<CancellationToken>()))
            .Callback<GetExchangeRatesQuery, CancellationToken>((q, _) => capturedQuery = q)
            .ReturnsAsync(dto);

        // Act
        await _sut.GetExchangeRates(null, "eur", CancellationToken.None);

        // Assert
        capturedQuery!.Currency.Should().Be("EUR");
    }
}
}
