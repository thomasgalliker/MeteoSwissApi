namespace MeteoSwissApi.Tests
{
    /// <summary>
    /// The tests in this file call the live SwissMetNet files.
    /// </summary>
    [Collection(Collections.IntegrationTests)]
    public partial class SwissMetNetServiceTests
    {
        private readonly MeteoSwissApiOptions options;
        private readonly ITestOutputHelper testOutputHelper;
        private readonly DumpOptions dumpOptions;

        [Fact]
        [Trait(Traits.Category, Traits.IntegrationTests)]
        public async Task GetWeatherStationsAsync_Live_ReturnsStations()
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
        [Trait(Traits.Category, Traits.IntegrationTests)]
        public async Task GetWeatherStationAsync_Live_ReturnsStation()
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
        [Trait(Traits.Category, Traits.IntegrationTests)]
        public async Task GetLatestMeasurementsAsync_Live_ReturnsMeasurements()
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
        [Trait(Traits.Category, Traits.IntegrationTests)]
        public async Task GetLatestMeasurementAsync_Live_ReturnsMeasurement()
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