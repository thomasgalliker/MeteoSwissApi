using System.Threading.Tasks;
using FluentAssertions;
using MeteoSwissApi;
using MeteoSwissApi.Models;
using MeteoSwissApi.Tests.Logging;
using MeteoSwissApi.Tests.Utils;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace WeatherDisplay.Tests.Services.MeteoSwiss
{
    public class MeteoSwissWeatherServiceIntegrationTests
    {
        private const string IconFileExtension = "svg";

        private readonly ILogger<MeteoSwissWeatherService> logger;
        private readonly IMeteoSwissWeatherServiceConfiguration meteoSwissWeatherServiceConfiguration;
        private readonly ITestOutputHelper testOutputHelper;
        private readonly DumpOptions dumpOptions;
        private readonly TestHelper testHelper;

        public MeteoSwissWeatherServiceIntegrationTests(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
            this.logger = new TestOutputHelperLogger<MeteoSwissWeatherService>(testOutputHelper);
            this.meteoSwissWeatherServiceConfiguration = new MeteoSwissWeatherServiceConfiguration
            {
                VerboseLogging = true
            };

            this.dumpOptions = new DumpOptions
            {
                DumpStyle = DumpStyle.CSharp,
                SetPropertiesOnly = true
            };

            this.dumpOptions.CustomInstanceFormatters.AddFormatter<Temperature>(t => $"new Temperature({t.Value}, {nameof(TemperatureUnit)}.{t.Unit})");

            this.testHelper = new TestHelper(testOutputHelper);
        }

        [Theory]
        [InlineData(6330)]
        [InlineData(633000)]
        public async Task ShouldGetCurrentWeatherAsync(int plz)
        {
            // Arrange
            IMeteoSwissWeatherService meteoSwissWeatherService = new MeteoSwissWeatherService(this.logger, this.meteoSwissWeatherServiceConfiguration);

            // Act
            var weatherInfo = await meteoSwissWeatherService.GetCurrentWeatherAsync(plz);

            // Assert
            this.testOutputHelper.WriteLine(ObjectDumper.Dump(weatherInfo, this.dumpOptions));

            weatherInfo.Should().NotBeNull();
        }

        [Fact]
        public async Task ShouldGetWeatherIconAsync_6330()
        {
            // Arrange
            const int plz = 6330;

            IMeteoSwissWeatherService meteoSwissWeatherService = new MeteoSwissWeatherService(this.logger, this.meteoSwissWeatherServiceConfiguration);
            var weatherInfo = await meteoSwissWeatherService.GetCurrentWeatherAsync(plz);

            // Act
            var iconStream = await meteoSwissWeatherService.GetWeatherIconAsync(weatherInfo.CurrentWeather.IconV2);

            // Assert
            iconStream.Should().NotBeNull();
            this.testHelper.WriteFile(iconStream, IconFileExtension);
        }
    }
}