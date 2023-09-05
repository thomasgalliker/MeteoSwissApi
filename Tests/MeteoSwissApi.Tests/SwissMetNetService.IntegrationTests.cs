using System;
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
    public class SwissMetNetServiceIntegrationTests
    {
        private readonly ILogger<SwissMetNetService> logger;
        private readonly MeteoSwissApiOptions options;
        private readonly ITestOutputHelper testOutputHelper;
        private readonly DumpOptions dumpOptions;

        public SwissMetNetServiceIntegrationTests(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
            this.logger = new TestOutputHelperLogger<SwissMetNetService>(testOutputHelper);
            this.options = new MeteoSwissApiOptions
            {
                VerboseLogging = true
            };

            this.dumpOptions = new DumpOptions
            {
                DumpStyle = DumpStyle.CSharp,
                SetPropertiesOnly = true
            };

            this.dumpOptions.CustomInstanceFormatters.AddFormatter<Temperature>(t => $"Temperature.FromDegreesCelsius({t.Value})");
            this.dumpOptions.CustomInstanceFormatters.AddFormatter<Length>(t => $"Length.FromMeters({t.Value})");
        }

        [Fact]
        public async Task ShouldGetWeatherStationsAsync()
        {
            // Arrange
            TimeSpan? cacheExpiration = null;

            ISwissMetNetService swissMetNetService = new SwissMetNetService(this.logger, this.options);

            // Act
            var weatherStations = await swissMetNetService.GetWeatherStationsAsync(cacheExpiration);

            // Assert
            this.testOutputHelper.WriteLine(ObjectDumper.Dump(weatherStations, this.dumpOptions));

            weatherStations.Should().NotBeEmpty();
        }

        [Fact]
        public async Task ShouldGetWeatherStationAsync_ByStationCode()
        {
            // Arrange
            const string stationCode = "CHZ";
            TimeSpan? cacheExpiration = null;

            ISwissMetNetService swissMetNetService = new SwissMetNetService(this.logger, this.options);

            // Act
            var weatherStation = await swissMetNetService.GetWeatherStationAsync(stationCode, cacheExpiration);

            // Assert
            this.testOutputHelper.WriteLine(ObjectDumper.Dump(weatherStation, this.dumpOptions));

            weatherStation.Should().BeEquivalentTo(
                new WeatherStation
                {
                    Place = "Cham",
                    StationCode = "CHZ",
                    WigosId = "0-20000-0-06674",
                    StationType = WeatherStationType.WeatherStation,
                    BarometricAltitude = Length.FromMeters(443m),
                    Location = new GeoCoordinate(47.188278d, 8.464642d, Length.FromMeters(443m)),
                    DataOwners = new[] { "MeteoSwiss" },
                    Canton = "ZG"
                });
        }

        [Fact]
        public async Task ShouldGetLatestMeasurementsAsync()
        {
            // Arrange
            TimeSpan? cacheExpiration = null;

            ISwissMetNetService swissMetNetService = new SwissMetNetService(this.logger, this.options);

            // Act
            var measurements = await swissMetNetService.GetLatestMeasurementsAsync(cacheExpiration);

            // Assert
            this.testOutputHelper.WriteLine(ObjectDumper.Dump(measurements, this.dumpOptions));

            measurements.Should().NotBeEmpty();
        }

        [Fact]
        public async Task ShouldGetLatestMeasurementAsync_ByStationCode()
        {
            // Arrange
            const string stationCode = "CHZ";
            TimeSpan? cacheExpiration = null;

            ISwissMetNetService swissMetNetService = new SwissMetNetService(this.logger, this.options);

            // Act
            var measurement = await swissMetNetService.GetLatestMeasurementAsync(stationCode, cacheExpiration);

            // Assert
            this.testOutputHelper.WriteLine(ObjectDumper.Dump(measurement, this.dumpOptions));

            measurement.Should().NotBeNull();
        }
    }
}