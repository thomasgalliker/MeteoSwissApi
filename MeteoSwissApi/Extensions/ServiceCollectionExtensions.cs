using System;
using MeteoSwissApi;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMeteoSwissApi(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            // Configuration
            serviceCollection.Configure<MeteoSwissApiOptions>(configuration);

            // Register services
            serviceCollection.AddMeteoSwissApi();

            return serviceCollection;
        }

        public static IServiceCollection AddMeteoSwissApi(
            this IServiceCollection services,
            Action<MeteoSwissApiOptions> options = null)
        {
            // Configuration
            if (options != null)
            {
                services.Configure(options);
            }

            // Register services
            services.AddSingleton<IMeteoSwissWeatherService, MeteoSwissWeatherService>();
            services.AddSingleton<ISwissMetNetService, SwissMetNetService>();
            services.AddSingleton<ISlfDataService, SlfDataService>();

            return services;
        }
    }
}