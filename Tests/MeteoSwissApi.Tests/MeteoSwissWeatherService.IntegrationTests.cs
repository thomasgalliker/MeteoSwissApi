using System.Threading.Tasks;
using FluentAssertions;
using MeteoSwissApi.Models;
using MeteoSwissApi.Tests.Logging;
using MeteoSwissApi.Tests.Utils;
using Microsoft.Extensions.Logging;
using UnitsNet;
using Xunit;
using Xunit.Abstractions;

namespace MeteoSwissApi.Tests
{
    public class MeteoSwissWeatherServiceIntegrationTests
    {
        private const string IconFileExtension = "svg";

        private readonly ILogger<MeteoSwissWeatherService> logger;
        private readonly IMeteoSwissWeatherServiceOptions options;
        private readonly ITestOutputHelper testOutputHelper;
        private readonly DumpOptions dumpOptions;
        private readonly TestHelper testHelper;

        public MeteoSwissWeatherServiceIntegrationTests(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
            this.logger = new TestOutputHelperLogger<MeteoSwissWeatherService>(testOutputHelper);
            this.options = new MeteoSwissWeatherServiceOptions
            {
                VerboseLogging = true
            };

            this.dumpOptions = new DumpOptions
            {
                DumpStyle = DumpStyle.CSharp,
                SetPropertiesOnly = true
            };

            this.dumpOptions.CustomInstanceFormatters.AddFormatter<Temperature>(t => $"Temperature.FromDegreesCelsius({t.Value})");

            this.testHelper = new TestHelper(testOutputHelper);
        }

        [Theory]
        [InlineData(6330)]
        [InlineData(633000)]
        [InlineData(690000)]
        [InlineData(601000)]
        [InlineData(671700)]
        [InlineData(195000)]
        [InlineData(774200)]
        public async Task ShouldGetCurrentWeatherAsync(int plz)
        {
            // Arrange
            IMeteoSwissWeatherService meteoSwissWeatherService = new MeteoSwissWeatherService(this.logger, this.options);

            // Act
            var weatherInfo = await meteoSwissWeatherService.GetCurrentWeatherAsync(plz);

            // Assert
            this.testOutputHelper.WriteLine(ObjectDumper.Dump(weatherInfo, this.dumpOptions));

            weatherInfo.Should().NotBeNull();
        }

        [Theory]
        [InlineData(6330)]
        [InlineData(633000)]
        [InlineData(690000)]
        [InlineData(601000)]
        [InlineData(671700)]
        [InlineData(195000)]
        [InlineData(774200)]
        public async Task ShouldGetForecastAsync(int plz)
        {
            // Arrange
            IMeteoSwissWeatherService meteoSwissWeatherService = new MeteoSwissWeatherService(this.logger, this.options);

            // Act
            var forecastInfo = await meteoSwissWeatherService.GetForecastAsync(plz);

            // Assert
            this.testOutputHelper.WriteLine(ObjectDumper.Dump(forecastInfo, this.dumpOptions));

            forecastInfo.Should().NotBeNull();
        }

        [Theory]
        [InlineData("en")]
        [InlineData("de")]
        [InlineData("fr")]
        [InlineData("it")]
        public async Task ShouldGetCurrentWeatherAsync_WithLanguage(string language)
        {
            // Arrange
            const int plz = 774200;

            var configuration = new MeteoSwissWeatherServiceOptions
            {
                Language = language,
                VerboseLogging = true
            };

            IMeteoSwissWeatherService meteoSwissWeatherService = new MeteoSwissWeatherService(this.logger, configuration);

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

            IMeteoSwissWeatherService meteoSwissWeatherService = new MeteoSwissWeatherService(this.logger, this.options);
            var weatherInfo = await meteoSwissWeatherService.GetCurrentWeatherAsync(plz);

            // Act
            var iconStream = await meteoSwissWeatherService.GetWeatherIconAsync(weatherInfo.CurrentWeather.IconV2);

            // Assert
            iconStream.Should().NotBeNull();
            this.testHelper.WriteFile(iconStream, IconFileExtension);
        }
    }
}