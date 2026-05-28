using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using ExchangeRates.Application.Interfaces;
using ExchangeRates.Infrastructure.Configuration;
using ExchangeRates.Infrastructure.HttpClients;
using ExchangeRates.Infrastructure.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ExchangeRates.Infrastructure.Extensions
{

/// <summary>
/// Extension methods for registering Infrastructure-layer services with the DI container.
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Bind options from configuration
        services
            .AddOptions<CnbApiOptions>()
            .Bind(configuration.GetSection(CnbApiOptions.SectionName))
            .ValidateDataAnnotations();

        var cnbOptions = configuration
            .GetSection(CnbApiOptions.SectionName)
            .Get<CnbApiOptions>() ?? new CnbApiOptions();

        // Register typed HTTP client
        services
            .AddHttpClient<CnbApiClient>(client =>
            {
                client.BaseAddress = new Uri(cnbOptions.BaseUrl);
                client.Timeout = TimeSpan.FromSeconds(cnbOptions.TimeoutSeconds);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("User-Agent", "ExchangeRatesApp/1.0");
            });

        // Register provider
        services.AddScoped<IExchangeRateProvider, ExchangeRateProvider>();

        return services;
    }
}
}
