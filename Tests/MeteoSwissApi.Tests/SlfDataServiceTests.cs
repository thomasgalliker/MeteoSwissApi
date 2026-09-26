using System.Text.Json;

namespace MeteoSwissApi.Tests
{
    [Trait(Traits.Category, Traits.UnitTests)]
    public class SlfDataServiceTests
    {
        private const string EmptyFeatureCollectionJson = "{\"type\":\"FeatureCollection\",\"features\":[]}";
        private const string Network = "IMIS";
        private const string StationCode = "ZNZ1";

        private static readonly DateTime Timestamp = new DateTime(2026, 9, 26, 16, 0, 0, DateTimeKind.Utc);

        private readonly ILogger<SlfDataService> logger;

        public SlfDataServiceTests(ITestOutputHelper testOutputHelper)
        {
            this.logger = new TestOutputHelperLogger<SlfDataService>(testOutputHelper);
        }

        [Fact]
        public async Task GetLatestMeasurementsAsync_WindSpeedAndDirection_ReturnsWindSpeedAndDirection()
        {
            // Arrange
            var slfDataService = this.CreateSlfDataService(CreateWindMeanResponseJson(velocity: 5.31m, direction: 315.5m));

            // Act
            var measurements = await slfDataService.GetLatestMeasurementsAsync();

            // Assert
            var measurement = measurements.Should().ContainSingle().Which;
            measurement.WindSpeedMean.Date.Should().Be(Timestamp);
            measurement.WindSpeedMean.Value.Should().Be(Speed.FromKilometersPerHour(5.31));
            measurement.WindDirection.Date.Should().Be(Timestamp);
            measurement.WindDirection.Value.Should().Be(Angle.FromDegrees(315.5));
        }

        [Fact]
        public async Task GetLatestMeasurementsAsync_WindDirectionIsNull_ReturnsWindSpeedWithoutDirection()
        {
            // Arrange
            var slfDataService = this.CreateSlfDataService(CreateWindMeanResponseJson(velocity: 16.128m, direction: null));

            // Act
            var measurements = await slfDataService.GetLatestMeasurementsAsync();

            // Assert
            var measurement = measurements.Should().ContainSingle().Which;
            measurement.WindSpeedMean.Date.Should().Be(Timestamp);
            measurement.WindSpeedMean.Value.Should().Be(Speed.FromKilometersPerHour(16.128));
            measurement.WindDirection.Date.Should().Be(default(DateTime));
        }

        [Fact]
        public async Task GetLatestMeasurementsAsync_WindSpeedIsNull_ReturnsWindDirectionWithoutSpeed()
        {
            // Arrange
            var slfDataService = this.CreateSlfDataService(CreateWindMeanResponseJson(velocity: null, direction: 248.3m));

            // Act
            var measurements = await slfDataService.GetLatestMeasurementsAsync();

            // Assert
            var measurement = measurements.Should().ContainSingle().Which;
            measurement.WindSpeedMean.Date.Should().Be(default(DateTime));
            measurement.WindDirection.Date.Should().Be(Timestamp);
            measurement.WindDirection.Value.Should().Be(Angle.FromDegrees(248.3));
        }

        [Fact]
        public async Task GetMeasurementsByStationCodeAsync_WindDirectionIsNull_ReturnsWindSpeedWithoutDirection()
        {
            // Arrange
            var slfDataService = this.CreateSlfDataServiceWithTimeseries(
                CreateTimeseriesResponseJson(velocityMax: 22.3m, velocityMean: 16.128m, direction: null));

            // Act
            var measurements = await slfDataService.GetMeasurementsByStationCodeAsync(Network, StationCode);

            // Assert
            var wind = measurements.Should().ContainSingle().Which.Wind;
            wind.VelocityMax.Should().Be(Speed.FromKilometersPerHour(22.3));
            wind.VelocityMean.Should().Be(Speed.FromKilometersPerHour(16.128));
            wind.Direction.Should().BeNull();
        }

        [Fact]
        public async Task GetMeasurementsByStationCodeAsync_WindSpeedIsNull_ReturnsWindDirectionWithoutSpeed()
        {
            // Arrange
            var slfDataService = this.CreateSlfDataServiceWithTimeseries(
                CreateTimeseriesResponseJson(velocityMax: null, velocityMean: null, direction: 248.3m));

            // Act
            var measurements = await slfDataService.GetMeasurementsByStationCodeAsync(Network, StationCode);

            // Assert
            var wind = measurements.Should().ContainSingle().Which.Wind;
            wind.VelocityMax.Should().BeNull();
            wind.VelocityMean.Should().BeNull();
            wind.Direction.Should().Be(Angle.FromDegrees(248.3));
        }

        [Fact]
        public async Task GetMeasurementsByStationCodeAsync_WindTimestampIsMissing_ReturnsNullWindValues()
        {
            // Arrange
            var missingWindTimestamp = Timestamp.AddMinutes(30);
            var responseJson = JsonSerializer.Serialize(new
            {
                windVelocityMax = new[] { new { timestamp = Timestamp, value = 22.3m } },
                windVelocityMean = new[] { new { timestamp = Timestamp, value = 16.128m } },
                windDirectionMean = new[] { new { timestamp = Timestamp, value = 315.5m } },
                temperatureAir = new[]
                {
                    new { timestamp = Timestamp, value = 5.316m },
                    new { timestamp = missingWindTimestamp, value = 5.1m },
                },
            });
            var slfDataService = this.CreateSlfDataServiceWithTimeseries(responseJson);

            // Act
            var measurements = await slfDataService.GetMeasurementsByStationCodeAsync(Network, StationCode);

            // Assert
            var wind = measurements.Should().ContainSingle(m => m.Date == missingWindTimestamp).Which.Wind;
            wind.VelocityMax.Should().BeNull();
            wind.VelocityMean.Should().BeNull();
            wind.Direction.Should().BeNull();
        }

        private SlfDataService CreateSlfDataService(string windMeanResponseJson)
        {
            // Only the WIND_MEAN endpoint returns a station, all other endpoints return no features.
            var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync((HttpRequestMessage request, CancellationToken _) => new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(request.RequestUri.AbsolutePath.Contains("/WIND_MEAN/")
                        ? windMeanResponseJson
                        : EmptyFeatureCollectionJson),
                });

            var httpClient = new HttpClient(httpMessageHandlerMock.Object);
            return new SlfDataService(this.logger, httpClient, new MeteoSwissApiOptions());
        }

        private static string CreateWindMeanResponseJson(decimal? velocity, decimal? direction)
        {
            var response = new
            {
                type = "FeatureCollection",
                features = new[]
                {
                    new
                    {
                        type = "Feature",
                        geometry = new { type = "Point", coordinates = new[] { 9.9976221513, 46.7042518299, 3133 } },
                        properties = new
                        {
                            network = "IMIS",
                            code = "ZNZ1",
                            elevation = 3133,
                            label = "Sarsura Pitschen",
                            type = "WIND",
                            manual = false,
                            velocity,
                            direction,
                            timestamp = Timestamp,
                        },
                    },
                },
            };

            return JsonSerializer.Serialize(response);
        }

        private SlfDataService CreateSlfDataServiceWithTimeseries(string timeseriesResponseJson)
        {
            var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(() => new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(timeseriesResponseJson),
                });

            var httpClient = new HttpClient(httpMessageHandlerMock.Object);
            return new SlfDataService(this.logger, httpClient, new MeteoSwissApiOptions());
        }

        private static string CreateTimeseriesResponseJson(decimal? velocityMax, decimal? velocityMean, decimal? direction)
        {
            var response = new
            {
                windVelocityMax = new[] { new { timestamp = Timestamp, value = velocityMax } },
                windVelocityMean = new[] { new { timestamp = Timestamp, value = velocityMean } },
                windDirectionMean = new[] { new { timestamp = Timestamp, value = direction } },
                temperatureAir = new[] { new { timestamp = Timestamp, value = 5.316m } },
            };

            return JsonSerializer.Serialize(response);
        }
    }
}
