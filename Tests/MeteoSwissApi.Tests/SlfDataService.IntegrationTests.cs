namespace MeteoSwissApi.Tests
{
    /// <summary>
    /// The tests in this file call the live SLF services.
    /// </summary>
    [Collection(Collections.IntegrationTests)]
    public partial class SlfDataServiceTests
    {
        private readonly MeteoSwissApiOptions options;
        private readonly ITestOutputHelper testOutputHelper;
        private readonly DumpOptions dumpOptions;

        [Fact]
        [Trait(Traits.Category, Traits.IntegrationTests)]
        public async Task GetLatestMeasurementsAsync_Live_ReturnsMeasurements()
        {
            // Arrange
            ISlfDataService slfDataService = new SlfDataService(this.logger, this.options);

            // Act
            var measurements = (await slfDataService.GetLatestMeasurementsAsync())
                .OrderBy(m => m.Station.Code)
                .ToArray();

            // Assert
            this.testOutputHelper.WriteLine(ObjectDumper.Dump(measurements.Take(10), this.dumpOptions));

            measurements.Should().NotBeEmpty();
            measurements.Should().Contain(m => !string.IsNullOrWhiteSpace(m.Station.Network));
            measurements.Should().Contain(m => !string.IsNullOrWhiteSpace(m.Station.Code));
            measurements.Should().Contain(m => !string.IsNullOrWhiteSpace(m.Station.Type.Value));
            measurements.Should().Contain(m => m.SnowHeight != null || m.AirTemperature != null || m.WindSpeedMean != null);
        }

        [Fact]
        [Trait(Traits.Category, Traits.IntegrationTests)]
        public async Task GetMeasurementsByStationCodeAsync_Live_ReturnsMeasurements()
        {
            // Arrange
            const string network = "IMIS";
            const string stationCode = "DAV2";

            ISlfDataService slfDataService = new SlfDataService(this.logger, this.options);

            // Act
            var measurements = (await slfDataService.GetMeasurementsByStationCodeAsync(network, stationCode))
                .ToArray();

            // Assert
            this.testOutputHelper.WriteLine(ObjectDumper.Dump(measurements.Take(5), this.dumpOptions));

            measurements.Should().NotBeEmpty();
            measurements.Should().BeInAscendingOrder(m => m.Date);
            measurements.Should().OnlyContain(m => m.Date.Kind == DateTimeKind.Utc);

            // Snow height and snow surface temperature share the air temperature time grid.
            measurements.Should().Contain(m => m.SnowHeight.HasValue);
            measurements.Should().Contain(m => m.SurfaceTemperature.HasValue);

            // New snow height is reported on a coarser (daily) grid, so only a few items carry a
            // value while the rest stay null.
            measurements.Should().Contain(m => m.NewSnowHeight.HasValue);
            measurements.Should().Contain(m => !m.NewSnowHeight.HasValue);
        }
    }
}
