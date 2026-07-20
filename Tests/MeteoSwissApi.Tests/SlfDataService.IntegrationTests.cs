namespace MeteoSwissApi.Tests
{
    public class SlfDataServiceIntegrationTests
    {
        private readonly ILogger<SlfDataService> logger;
        private readonly MeteoSwissApiOptions options;
        private readonly ITestOutputHelper testOutputHelper;
        private readonly DumpOptions dumpOptions;

        public SlfDataServiceIntegrationTests(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
            this.logger = new TestOutputHelperLogger<SlfDataService>(testOutputHelper);
            this.options = new MeteoSwissApiOptions
            {
                VerboseLogging = true
            };

            this.dumpOptions = new DumpOptions
            {
                DumpStyle = DumpStyle.CSharp,
                SetPropertiesOnly = true
            };

            this.dumpOptions.CustomInstanceFormatters.AddFormatter<Angle>(a => $"Angle.FromDegrees({a.Value})");
            this.dumpOptions.CustomInstanceFormatters.AddFormatter<Length>(l => $"Length.FromMeters({l.Value})");
            this.dumpOptions.CustomInstanceFormatters.AddFormatter<Speed>(s => $"Speed.FromKilometersPerHour({s.Value})");
            this.dumpOptions.CustomInstanceFormatters.AddFormatter<Temperature>(t => $"Temperature.FromDegreesCelsius({t.Value})");
        }

        [Fact]
        public async Task ShouldGetLatestMeasurementsAsync()
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
            measurements.Should().Contain(m => m.SnowHeight.Date != default || m.AirTemperature.Date != default || m.WindSpeedMean.Date != default);
        }

        [Fact]
        public async Task ShouldGetMeasurementsByStationCodeAsync()
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
