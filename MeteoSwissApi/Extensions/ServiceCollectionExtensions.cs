using MeteoSwissApi;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers <see cref="IMeteoSwissWeatherService"/>, <see cref="ISwissMetNetService"/> and <see cref="ISlfDataService"/> as singletons,
        /// with <see cref="MeteoSwissApiOptions"/> bound to <paramref name="configuration"/>.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configuration">
        /// The configuration section that holds the options, for example the section <c>MeteoSwissApi</c> of appsettings.json:
        /// <code>
        /// "MeteoSwissApi": {
        ///   "Language": "de",
        ///   "VerboseLogging": false,
        ///   "SwissMetNet": {
        ///     "CacheExpiration": "00:20:00"
        ///   }
        /// }
        /// </code>
        /// </param>
        /// <remarks>
        /// The services log through <see cref="Microsoft.Extensions.Logging.ILogger{TCategoryName}"/> if logging is registered.
        /// Invalid options, such as a language other than de, fr, it or en, fail with <see cref="OptionsValidationException"/>
        /// when a service is first resolved. Calling this method more than once registers the services only once.
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="services"/> or <paramref name="configuration"/> is null.</exception>
        public static IServiceCollection AddMeteoSwissApi(this IServiceCollection services, IConfiguration configuration)
        {
            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            services.Configure<MeteoSwissApiOptions>(configuration);
            return services.AddMeteoSwissApi();
        }

        /// <summary>
        /// Registers <see cref="IMeteoSwissWeatherService"/>, <see cref="ISwissMetNetService"/> and <see cref="ISlfDataService"/> as singletons,
        /// with <see cref="MeteoSwissApiOptions"/> set up by <paramref name="configureOptions"/>.
        /// </summary>
        /// <inheritdoc cref="AddMeteoSwissApi(IServiceCollection, IConfiguration)" path="/remarks"/>
        /// <exception cref="ArgumentNullException"><paramref name="services"/> or <paramref name="configureOptions"/> is null.</exception>
        public static IServiceCollection AddMeteoSwissApi(this IServiceCollection services, Action<MeteoSwissApiOptions> configureOptions)
        {
            if (configureOptions is null)
            {
                throw new ArgumentNullException(nameof(configureOptions));
            }

            services.Configure(configureOptions);
            return services.AddMeteoSwissApi();
        }

        /// <summary>
        /// Registers <see cref="IMeteoSwissWeatherService"/>, <see cref="ISwissMetNetService"/> and <see cref="ISlfDataService"/> as singletons
        /// with the default <see cref="MeteoSwissApiOptions"/>.
        /// </summary>
        /// <inheritdoc cref="AddMeteoSwissApi(IServiceCollection, IConfiguration)" path="/remarks"/>
        /// <exception cref="ArgumentNullException"><paramref name="services"/> is null.</exception>
        public static IServiceCollection AddMeteoSwissApi(this IServiceCollection services)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddOptions();
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IValidateOptions<MeteoSwissApiOptions>, MeteoSwissApiOptionsValidator>());

            // Each service is created through one named constructor, so that the container never has to choose between the constructors.
            services.TryAddSingleton<IMeteoSwissWeatherService>(provider => new MeteoSwissWeatherService(GetLogger<MeteoSwissWeatherService>(provider), GetOptions(provider)));
            services.TryAddSingleton<ISwissMetNetService>(provider => new SwissMetNetService(GetLogger<SwissMetNetService>(provider), GetOptions(provider)));
            services.TryAddSingleton<ISlfDataService>(provider => new SlfDataService(GetLogger<SlfDataService>(provider), GetOptions(provider)));

            return services;
        }

        private static ILogger<T> GetLogger<T>(IServiceProvider provider)
        {
            return provider.GetService<ILogger<T>>() ?? NullLogger<T>.Instance;
        }

        private static IOptions<MeteoSwissApiOptions> GetOptions(IServiceProvider provider)
        {
            return provider.GetRequiredService<IOptions<MeteoSwissApiOptions>>();
        }
    }
}