using MeteoSwissApi;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMeteoSwissApi(this IServiceCollection serviceCollection)
        {
            // Configuration
            // TODO

            // Register services
            serviceCollection.AddSingleton<IMeteoSwissWeatherServiceOptions, MeteoSwissWeatherServiceOptions>();
            serviceCollection.AddSingleton<IMeteoSwissWeatherService, MeteoSwissWeatherService>();

            serviceCollection.AddSingleton<ISwissMetNetServiceOptions, SwissMetNetServiceOptions>();
            serviceCollection.AddSingleton<ISwissMetNetService, SwissMetNetService>();

            serviceCollection.AddSingleton<ISlfDataServiceOptions, SlfDataServiceOptions>();
            serviceCollection.AddSingleton<ISlfDataService, SlfDataService>();

            return serviceCollection;
        }
    }
}