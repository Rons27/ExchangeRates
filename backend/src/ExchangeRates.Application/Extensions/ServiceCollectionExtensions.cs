using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using ExchangeRates.Application.Interfaces;
using ExchangeRates.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ExchangeRates.Application.Extensions
{

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IExchangeRateService, ExchangeRateService>();
        return services;
    }
}
}
