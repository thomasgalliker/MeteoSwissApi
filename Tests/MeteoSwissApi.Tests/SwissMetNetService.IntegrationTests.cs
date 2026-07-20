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
            this.dumpOptions.CustomInstanceFormatters.AddFormatter<Duration>(t => $"{t.Value} (Duration)");
            this.dumpOptions.CustomInstanceFormatters.AddFormatter<Irradiance>(t => $"{t.Value} (Irradiance)");
            this.dumpOptions.CustomInstanceFormatters.AddFormatter<RelativeHumidity>(t => $"{t.Value} (RelativeHumidity)");
            this.dumpOptions.CustomInstanceFormatters.AddFormatter<Angle>(t => $"{t.Value} (Angle)");
            this.dumpOptions.CustomInstanceFormatters.AddFormatter<Speed>(t => $"{t.Value} (Speed)");
            this.dumpOptions.CustomInstanceFormatters.AddFormatter<Pressure>(t => $"{t.Value} (Pressure)");
        }

        [Fact]
        public async Task ShouldGetWeatherStationsAsync()
        {
            // Arrange
            ISwissMetNetService swissMetNetService = new SwissMetNetService(this.logger, this.options);

            // Act
            var weatherStations = await swissMetNetService.GetWeatherStationsAsync(cacheExpiration: null);

            // Assert
            this.testOutputHelper.WriteLine(ObjectDumper.Dump(weatherStations, this.dumpOptions));

            weatherStations.Should().NotBeEmpty();
        }

        [Fact]
        public async Task ShouldGetWeatherStationAsync_ByStationCode()
        {
            // Arrange
            const string stationCode = "CHZ";

            ISwissMetNetService swissMetNetService = new SwissMetNetService(this.logger, this.options);

            // Act
            var weatherStation = await swissMetNetService.GetWeatherStationAsync(stationCode, cacheExpiration: null);

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
            ISwissMetNetService swissMetNetService = new SwissMetNetService(this.logger, this.options);

            // Act
            var measurements = await swissMetNetService.GetLatestMeasurementsAsync(cacheExpiration: null);

            // Assert
            this.testOutputHelper.WriteLine(ObjectDumper.Dump(measurements, this.dumpOptions));

            measurements.Should().NotBeEmpty();
        }

        [Fact]
        public async Task ShouldGetLatestMeasurementAsync_ByStationCode()
        {
            // Arrange
            const string stationCode = "CHZ";

            ISwissMetNetService swissMetNetService = new SwissMetNetService(this.logger, this.options);

            // Act
            var measurement = await swissMetNetService.GetLatestMeasurementAsync(stationCode, cacheExpiration: null);

            // Assert
            this.testOutputHelper.WriteLine(ObjectDumper.Dump(measurement, this.dumpOptions));

            measurement.Should().NotBeNull();
        }
    }
}