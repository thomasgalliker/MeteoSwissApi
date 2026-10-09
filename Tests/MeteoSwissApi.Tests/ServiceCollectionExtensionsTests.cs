using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace MeteoSwissApi.Tests
{
    [Trait(Traits.Category, Traits.UnitTests)]
    public class ServiceCollectionExtensionsTests
    {
        private readonly ITestOutputHelper testOutputHelper;

        public ServiceCollectionExtensionsTests(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void AddMeteoSwissApi_WithoutLogging_ResolvesAllServices()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddMeteoSwissApi();

            // Act
            using var serviceProvider = services.BuildServiceProvider();

            // Assert
            serviceProvider.GetRequiredService<IMeteoSwissWeatherService>().Should().BeOfType<MeteoSwissWeatherService>();
            serviceProvider.GetRequiredService<ISwissMetNetService>().Should().BeOfType<SwissMetNetService>();
            serviceProvider.GetRequiredService<ISlfDataService>().Should().BeOfType<SlfDataService>();
        }

        [Fact]
        public void AddMeteoSwissApi_WithLogging_ResolvesAllServices()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddSingleton<ILogger<MeteoSwissWeatherService>>(new TestOutputHelperLogger<MeteoSwissWeatherService>(this.testOutputHelper));
            services.AddSingleton<ILogger<SwissMetNetService>>(new TestOutputHelperLogger<SwissMetNetService>(this.testOutputHelper));
            services.AddSingleton<ILogger<SlfDataService>>(new TestOutputHelperLogger<SlfDataService>(this.testOutputHelper));
            services.AddMeteoSwissApi();

            // Act
            using var serviceProvider = services.BuildServiceProvider();

            // Assert
            serviceProvider.GetRequiredService<IMeteoSwissWeatherService>().Should().NotBeNull();
            serviceProvider.GetRequiredService<ISwissMetNetService>().Should().NotBeNull();
            serviceProvider.GetRequiredService<ISlfDataService>().Should().NotBeNull();
        }

        [Fact]
        public async Task AddMeteoSwissApi_ServiceProviderDisposed_DisposesTheHttpClientsOfTheServices()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddMeteoSwissApi();
            var serviceProvider = services.BuildServiceProvider();
            var meteoSwissWeatherService = serviceProvider.GetRequiredService<IMeteoSwissWeatherService>();
            var slfDataService = serviceProvider.GetRequiredService<ISlfDataService>();

            // Act
            serviceProvider.Dispose();

            // Assert
            await meteoSwissWeatherService.Invoking(s => s.GetForecastAsync(6330)).Should().ThrowAsync<ObjectDisposedException>();
            await slfDataService.Invoking(s => s.GetStationInfoAsync("IMIS", "DAV2")).Should().ThrowAsync<ObjectDisposedException>();
        }

        [Fact]
        public void AddMeteoSwissApi_CalledTwice_RegistersEachServiceOnce()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.AddMeteoSwissApi();
            services.AddMeteoSwissApi(o => o.Language = "de");

            // Assert
            services.Count(d => d.ServiceType == typeof(IMeteoSwissWeatherService)).Should().Be(1);
            services.Count(d => d.ServiceType == typeof(ISwissMetNetService)).Should().Be(1);
            services.Count(d => d.ServiceType == typeof(ISlfDataService)).Should().Be(1);
            services.Count(d => d.ServiceType == typeof(IValidateOptions<MeteoSwissApiOptions>)).Should().Be(1);
        }

        [Fact]
        public void AddMeteoSwissApi_Always_RegistersSingletons()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddMeteoSwissApi();
            using var serviceProvider = services.BuildServiceProvider();

            // Act
            var firstService = serviceProvider.GetRequiredService<ISwissMetNetService>();
            var secondService = serviceProvider.GetRequiredService<ISwissMetNetService>();

            // Assert
            secondService.Should().BeSameAs(firstService);
        }

        [Fact]
        public void AddMeteoSwissApi_OptionsRegisteredAsService_ResolvesWithoutAmbiguity()
        {
            // Arrange
            // With the options registered as a service, two constructors would fit the container; the registration names the one it uses.
            var services = new ServiceCollection();
            services.AddSingleton(new MeteoSwissApiOptions());
            services.AddMeteoSwissApi();

            // Act
            using var serviceProvider = services.BuildServiceProvider();

            // Assert
            serviceProvider.GetRequiredService<IMeteoSwissWeatherService>().Should().NotBeNull();
        }

        [Fact]
        public void AddMeteoSwissApi_ConfigureOptions_UsesConfiguredOptions()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddMeteoSwissApi(o =>
            {
                o.Language = "it";
                o.SwissMetNet.CacheExpiration = TimeSpan.FromMinutes(20);
            });

            // Act
            using var serviceProvider = services.BuildServiceProvider();
            var options = serviceProvider.GetRequiredService<IOptions<MeteoSwissApiOptions>>().Value;

            // Assert
            options.Language.Should().Be("it");
            options.SwissMetNet.CacheExpiration.Should().Be(TimeSpan.FromMinutes(20));
        }

        [Fact]
        public void AddMeteoSwissApi_InvalidLanguage_ThrowsOptionsValidationExceptionOnResolve()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddMeteoSwissApi(o => o.Language = "es");
            using var serviceProvider = services.BuildServiceProvider();

            // Act
            Action action = () => serviceProvider.GetRequiredService<IMeteoSwissWeatherService>();

            // Assert
            action.Should().Throw<OptionsValidationException>().WithMessage("*Language must be one of: de, fr, it, en.*");
        }

        [Fact]
        public void AddMeteoSwissApi_ConfigureOptionsIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            Action action = () => services.AddMeteoSwissApi((Action<MeteoSwissApiOptions>)null);

            // Assert
            action.Should().Throw<ArgumentNullException>();
        }
        [Fact]
        public void AddMeteoSwissApi_ServicesIsNull_ThrowsArgumentNullException()
        {
            // Act
            Action action = () => ServiceCollectionExtensions.AddMeteoSwissApi(null);

            // Assert
            action.Should().Throw<ArgumentNullException>().WithParameterName("services");
        }

        [Fact]
        public void AddMeteoSwissApi_ConfigurationIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            Action action = () => services.AddMeteoSwissApi((IConfiguration)null);

            // Assert
            action.Should().Throw<ArgumentNullException>().WithParameterName("configuration");
        }
    }
}
