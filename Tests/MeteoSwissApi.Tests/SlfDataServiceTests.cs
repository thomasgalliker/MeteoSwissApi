using System.Collections.ObjectModel;
using System.Text.Json;

namespace MeteoSwissApi.Tests
{
    public partial class SlfDataServiceTests
    {
        private const string EmptyFeatureCollectionJson = "{\"type\":\"FeatureCollection\",\"features\":[]}";
        private const string Network = "IMIS";
        private const string StationCode = "ZNZ1";

        private static readonly DateTime Timestamp = new DateTime(2026, 9, 26, 16, 0, 0, DateTimeKind.Utc);

        private readonly ILogger<SlfDataService> logger;

        public SlfDataServiceTests(ITestOutputHelper testOutputHelper)
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
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetLatestMeasurementsAsync_WindSpeedAndDirection_ReturnsWindSpeedAndDirection()
        {
            // Arrange
            var slfDataService = this.CreateSlfDataServiceWithWindMean(CreateWindMeanResponseJson(velocity: 5.31m, direction: 315.5m));

            // Act
            var measurements = await slfDataService.GetLatestMeasurementsAsync();

            // Assert
            var measurement = measurements.Should().ContainSingle().Which;
            measurement.WindSpeedMean!.Date.Should().Be(Timestamp);
            measurement.WindSpeedMean.Value.Should().Be(Speed.FromKilometersPerHour(5.31));
            measurement.WindDirection!.Date.Should().Be(Timestamp);
            measurement.WindDirection.Value.Should().Be(Angle.FromDegrees(315.5));
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetLatestMeasurementsAsync_WindDirectionIsNull_ReturnsWindSpeedWithoutDirection()
        {
            // Arrange
            var slfDataService = this.CreateSlfDataServiceWithWindMean(CreateWindMeanResponseJson(velocity: 16.128m, direction: null));

            // Act
            var measurements = await slfDataService.GetLatestMeasurementsAsync();

            // Assert
            var measurement = measurements.Should().ContainSingle().Which;
            measurement.WindSpeedMean!.Date.Should().Be(Timestamp);
            measurement.WindSpeedMean.Value.Should().Be(Speed.FromKilometersPerHour(16.128));
            measurement.WindDirection.Should().BeNull();
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetLatestMeasurementsAsync_WindSpeedIsNull_ReturnsWindDirectionWithoutSpeed()
        {
            // Arrange
            var slfDataService = this.CreateSlfDataServiceWithWindMean(CreateWindMeanResponseJson(velocity: null, direction: 248.3m));

            // Act
            var measurements = await slfDataService.GetLatestMeasurementsAsync();

            // Assert
            var measurement = measurements.Should().ContainSingle().Which;
            measurement.WindSpeedMean.Should().BeNull();
            measurement.WindDirection!.Date.Should().Be(Timestamp);
            measurement.WindDirection.Value.Should().Be(Angle.FromDegrees(248.3));
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetLatestMeasurementsAsync_ParameterNotReported_ReturnsNull()
        {
            // Arrange
            var slfDataService = this.CreateSlfDataServiceWithWindMean(CreateWindMeanResponseJson(velocity: 5.31m, direction: 315.5m));

            // Act
            var measurements = await slfDataService.GetLatestMeasurementsAsync();

            // Assert
            var measurement = measurements.Should().ContainSingle().Which;
            measurement.AirTemperature.Should().BeNull();
            measurement.SnowHeight.Should().BeNull();
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetLatestMeasurementsAsync_WindStation_ReturnsNullWindSpeedMax()
        {
            // Arrange
            var slfDataService = this.CreateSlfDataServiceWithWindMean(CreateWindMeanResponseJson(velocity: 5.31m, direction: 315.5m));

            // Act
            var measurements = await slfDataService.GetLatestMeasurementsAsync();

            // Assert
            measurements.Should().ContainSingle().Which.WindSpeedMax.Should().BeNull();
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetLatestMeasurementsAsync_Feature_ReturnsStationOfFeature()
        {
            // Arrange
            var slfDataService = this.CreateSlfDataServiceWithWindMean(CreateWindMeanResponseJson(velocity: 5.31m, direction: 315.5m));

            // Act
            var measurements = await slfDataService.GetLatestMeasurementsAsync();

            // Assert
            var station = measurements.Should().ContainSingle().Which.Station;
            station.Network.Should().Be("IMIS");
            station.Code.Should().Be("ZNZ1");
            station.Name.Should().Be("Sarsura Pitschen");
            station.Type.Value.Should().Be(SlfStationType.Wind);
            station.Elevation.Should().Be(Length.FromMeters(3133));
            station.Manual.Should().BeFalse();
            station.Location.Type.Should().Be("Point");
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetLatestMeasurementsAsync_FeatureWithCoordinates_ReturnsStationLocation()
        {
            // Arrange
            var slfDataService = this.CreateSlfDataServiceWithWindMean(CreateWindMeanResponseJson(velocity: 5.31m, direction: 315.5m));

            // Act
            var measurements = await slfDataService.GetLatestMeasurementsAsync();

            // Assert
            var location = measurements.Should().ContainSingle().Which.Station.Location.Location;
            location.Should().Be(new GeoCoordinate(46.7042518299, 9.9976221513));
            location.Altitude.Should().Be(Length.FromMeters(3133));
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetLatestMeasurementsAsync_StationInTwoParameters_ReturnsOneMeasurementWithBothValues()
        {
            // Arrange
            var responsesByParameter = new Dictionary<string, string>
            {
                ["WIND_MEAN"] = CreateWindMeanResponseJson(velocity: 5.31m, direction: 315.5m),
                ["SNOW_HEIGHT"] = CreateFeatureCollectionJson(CreateFeature("IMIS", "ZNZ1", value: 120m, timestamp: Timestamp)),
            };
            var slfDataService = this.CreateSlfDataService(responsesByParameter);

            // Act
            var measurements = await slfDataService.GetLatestMeasurementsAsync();

            // Assert
            var measurement = measurements.Should().ContainSingle().Which;
            measurement.WindSpeedMean!.Value.Should().Be(Speed.FromKilometersPerHour(5.31));
            measurement.SnowHeight!.Value.Should().Be(Length.FromCentimeters(120));
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetLatestMeasurementsAsync_TwoStations_ReturnsTwoMeasurements()
        {
            // Arrange
            var responsesByParameter = new Dictionary<string, string>
            {
                ["WIND_MEAN"] = CreateWindMeanResponseJson(velocity: 5.31m, direction: 315.5m),
                ["SNOW_HEIGHT"] = CreateFeatureCollectionJson(CreateFeature("IMIS", "DAV2", value: 120m, timestamp: Timestamp)),
            };
            var slfDataService = this.CreateSlfDataService(responsesByParameter);

            // Act
            var measurements = await slfDataService.GetLatestMeasurementsAsync();

            // Assert
            measurements.Select(m => m.Station.Code).Should().BeEquivalentTo("ZNZ1", "DAV2");
            measurements.Single(m => m.Station.Code == "DAV2").SnowHeight!.Value.Should().Be(Length.FromCentimeters(120));
            measurements.Single(m => m.Station.Code == "DAV2").WindSpeedMean.Should().BeNull();
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetLatestMeasurementsAsync_Always_RequestsWindStationsForWindMeanOnly()
        {
            // Arrange
            var httpMessageHandlerMock = HttpMessageHandlerMocks.Returning(HttpStatusCode.OK, EmptyFeatureCollectionJson);
            var slfDataService = this.CreateSlfDataService(httpMessageHandlerMock.Object);

            // Act
            await slfDataService.GetLatestMeasurementsAsync();

            // Assert
            httpMessageHandlerMock.VerifySendAsync(Times.Once(), r => r.RequestUri!.AbsolutePath.Contains("/WIND_MEAN/") && r.RequestUri.Query == "?stationTypeFilter=WIND");
            httpMessageHandlerMock.VerifySendAsync(Times.Exactly(6), r => !r.RequestUri!.AbsolutePath.Contains("/WIND_MEAN/") && r.RequestUri.Query == string.Empty);
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetLatestMeasurementsAsync_StationsWithoutValue_LogsOneMessagePerParameter()
        {
            // Arrange
            // The live service lists manual stations without current value, with a null value and timestamp.
            var loggerMock = new Mock<ILogger<SlfDataService>>();
            var responsesByParameter = new Dictionary<string, string>
            {
                ["SNOW_HEIGHT"] = CreateFeatureCollectionJson(
                    CreateFeature("SMN", "*ABG0", value: null, timestamp: null),
                    CreateFeature("SMN", "*AIG0", value: null, timestamp: null)),
            };
            var httpClient = new HttpClient(CreateHttpMessageHandlerMock(responsesByParameter).Object);
            var slfDataService = new SlfDataService(loggerMock.Object, httpClient, new MeteoSwissApiOptions());

            // Act
            await slfDataService.GetLatestMeasurementsAsync();

            // Assert
            loggerMock.Verify(
                l => l.Log(
                    LogLevel.Debug,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((state, _) => $"{state}".Contains("2 stations reported no value for SNOW_HEIGHT")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetLatestMeasurementsAsync_Always_ReturnsReadOnlyList()
        {
            // Arrange
            var slfDataService = this.CreateSlfDataServiceWithWindMean(CreateWindMeanResponseJson(velocity: 5.31m, direction: 315.5m));

            // Act
            var measurements = await slfDataService.GetLatestMeasurementsAsync();

            // Assert
            measurements.Should().BeOfType<ReadOnlyCollection<SlfStationMeasurement>>();
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetLatestMeasurementByStationCodeAsync_StationWithoutValues_ReturnsNullValues()
        {
            // Arrange
            var slfDataService = this.CreateSlfDataServiceWithLatestMeasurement("{}");

            // Act
            var measurement = await slfDataService.GetLatestMeasurementByStationCodeAsync(Network, "DAV2");

            // Assert
            measurement.AirTemperature.Should().BeNull();
            measurement.SnowHeight.Should().BeNull();
            measurement.WindSpeedMean.Should().BeNull();
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetLatestMeasurementByStationCodeAsync_ValueIsNull_ReturnsNull()
        {
            // Arrange
            var slfDataService = this.CreateSlfDataServiceWithLatestMeasurement(CreateLatestMeasurementResponseJson(temperatureAir: null));

            // Act
            var measurement = await slfDataService.GetLatestMeasurementByStationCodeAsync(Network, "DAV2");

            // Assert
            measurement.AirTemperature.Should().BeNull();
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetLatestMeasurementByStationCodeAsync_ValuesArePresent_ReturnsValues()
        {
            // Arrange
            var slfDataService = this.CreateSlfDataServiceWithLatestMeasurement(CreateLatestMeasurementResponseJson(temperatureAir: 5.347m));

            // Act
            var measurement = await slfDataService.GetLatestMeasurementByStationCodeAsync(Network, "DAV2");

            // Assert
            measurement.Station.Code.Should().Be("DAV2");
            measurement.AirTemperature!.Date.Should().Be(Timestamp);
            measurement.AirTemperature.Date.Kind.Should().Be(DateTimeKind.Utc);
            measurement.AirTemperature.Value.Should().Be(Temperature.FromDegreesCelsius(5.347));
            measurement.SnowHeight!.Value.Should().Be(Length.FromCentimeters(120));
            measurement.WindDirection!.Value.Should().Be(Angle.FromDegrees(200.4));
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetLatestMeasurementByStationCodeAsync_Always_SendsBothRequestsAtOnce()
        {
            // Arrange
            // The first request is answered only once the second one has arrived, so sequential requests would time out.
            var secondRequestArrived = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            var requestCount = 0;
            var httpMessageHandlerMock = HttpMessageHandlerMocks.Returning(async (request, cancellationToken) =>
            {
                if (Interlocked.Increment(ref requestCount) == 1)
                {
                    await secondRequestArrived.Task.WaitAsync(TimeSpan.FromSeconds(10), cancellationToken);
                }
                else
                {
                    secondRequestArrived.TrySetResult(true);
                }

                var responseJson = request.RequestUri!.AbsolutePath.Contains("/info/")
                    ? CreateStationInfoResponseJson()
                    : CreateLatestMeasurementResponseJson(temperatureAir: 5.347m);

                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(responseJson) };
            });
            var slfDataService = this.CreateSlfDataService(httpMessageHandlerMock.Object);

            // Act
            var measurement = await slfDataService.GetLatestMeasurementByStationCodeAsync(Network, "DAV2");

            // Assert
            measurement.Station.Code.Should().Be("DAV2");
            requestCount.Should().Be(2);
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetMeasurementsByStationCodeAsync_WindDirectionIsNull_ReturnsWindSpeedWithoutDirection()
        {
            // Arrange
            var slfDataService = this.CreateSlfDataServiceWithResponse(
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
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetMeasurementsByStationCodeAsync_WindSpeedIsNull_ReturnsWindDirectionWithoutSpeed()
        {
            // Arrange
            var slfDataService = this.CreateSlfDataServiceWithResponse(
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
        [Trait(Traits.Category, Traits.UnitTests)]
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
            var slfDataService = this.CreateSlfDataServiceWithResponse(responseJson);

            // Act
            var measurements = await slfDataService.GetMeasurementsByStationCodeAsync(Network, StationCode);

            // Assert
            var wind = measurements.Should().ContainSingle(m => m.Date == missingWindTimestamp).Which.Wind;
            wind.VelocityMax.Should().BeNull();
            wind.VelocityMean.Should().BeNull();
            wind.Direction.Should().BeNull();
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetMeasurementsByStationCodeAsync_SnowValuesAreNull_ReturnsNullSnowValues()
        {
            // Arrange
            var slfDataService = this.CreateSlfDataServiceWithResponse(
                CreateSnowTimeseriesResponseJson(snowHeight: null, newSnowHeight: null, snowSurfaceTemperature: null));

            // Act
            var measurements = await slfDataService.GetMeasurementsByStationCodeAsync(Network, StationCode);

            // Assert
            var measurement = measurements.Should().ContainSingle().Which;
            measurement.SnowHeight.Should().BeNull();
            measurement.NewSnowHeight.Should().BeNull();
            measurement.SurfaceTemperature.Should().BeNull();
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetMeasurementsByStationCodeAsync_SnowValuesArePresent_ReturnsSnowValues()
        {
            // Arrange
            var slfDataService = this.CreateSlfDataServiceWithResponse(
                CreateSnowTimeseriesResponseJson(snowHeight: 120m, newSnowHeight: 15m, snowSurfaceTemperature: -5.2m));

            // Act
            var measurements = await slfDataService.GetMeasurementsByStationCodeAsync(Network, StationCode);

            // Assert
            var measurement = measurements.Should().ContainSingle().Which;
            measurement.SnowHeight.Should().Be(Length.FromCentimeters(120));
            measurement.NewSnowHeight.Should().Be(Length.FromCentimeters(15));
            measurement.SurfaceTemperature.Should().Be(Temperature.FromDegreesCelsius(-5.2));
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetMeasurementsByStationCodeAsync_StationWithoutAirTemperature_ReturnsSnowRows()
        {
            // Arrange
            var nextDay = Timestamp.AddDays(1);
            var responseJson = JsonSerializer.Serialize(new
            {
                snowHeight = new[]
                {
                    new { timestamp = Timestamp, value = 35m },
                    new { timestamp = nextDay, value = 40m },
                },
                heightNewSnow = new[]
                {
                    new { timestamp = Timestamp, value = 0m },
                    new { timestamp = nextDay, value = 5m },
                },
            });
            var slfDataService = this.CreateSlfDataServiceWithResponse(responseJson);

            // Act
            var measurements = (await slfDataService.GetMeasurementsByStationCodeAsync("BEOB", "1AD0")).ToArray();

            // Assert
            measurements.Select(m => m.Date).Should().Equal(Timestamp, nextDay);
            measurements[1].SnowHeight.Should().Be(Length.FromCentimeters(40));
            measurements[1].NewSnowHeight.Should().Be(Length.FromCentimeters(5));
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetMeasurementsByStationCodeAsync_AirTemperatureIsNull_ReturnsNullTemperatureAir()
        {
            // Arrange
            var responseJson = JsonSerializer.Serialize(new
            {
                temperatureAir = new[] { new { timestamp = Timestamp, value = (decimal?)null } },
            });
            var slfDataService = this.CreateSlfDataServiceWithResponse(responseJson);

            // Act
            var measurements = await slfDataService.GetMeasurementsByStationCodeAsync(Network, StationCode);

            // Assert
            measurements.Should().ContainSingle().Which.TemperatureAir.Should().BeNull();
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetMeasurementsByStationCodeAsync_SeriesIsNull_ReturnsRowsOfOtherSeries()
        {
            // Arrange
            const string responseJson = "{\"temperatureAir\":[{\"timestamp\":\"2026-09-26T16:00:00Z\",\"value\":5.316}],\"snowHeight\":null}";
            var slfDataService = this.CreateSlfDataServiceWithResponse(responseJson);

            // Act
            var measurements = await slfDataService.GetMeasurementsByStationCodeAsync(Network, StationCode);

            // Assert
            measurements.Should().ContainSingle().Which.SnowHeight.Should().BeNull();
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetMeasurementsByStationCodeAsync_TimestampWithOffset_ReturnsUtcDate()
        {
            // Arrange
            // SLF sends UTC timestamps (suffix Z). A timestamp with an offset is converted to UTC, not to local time.
            const string responseJson = "{\"temperatureAir\":[{\"timestamp\":\"2026-09-26T18:00:00+02:00\",\"value\":5.316}]}";
            var slfDataService = this.CreateSlfDataServiceWithResponse(responseJson);

            // Act
            var measurements = await slfDataService.GetMeasurementsByStationCodeAsync(Network, StationCode);

            // Assert
            var date = measurements.Should().ContainSingle().Which.Date;
            date.Should().Be(Timestamp);
            date.Kind.Should().Be(DateTimeKind.Utc);
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetMeasurementsByStationCodeAsync_Always_ReturnsReadOnlyList()
        {
            // Arrange
            var slfDataService = this.CreateSlfDataServiceWithResponse(
                CreateSnowTimeseriesResponseJson(snowHeight: 120m, newSnowHeight: 15m, snowSurfaceTemperature: -5.2m));

            // Act
            var measurements = await slfDataService.GetMeasurementsByStationCodeAsync(Network, StationCode);

            // Assert
            measurements.Should().BeOfType<ReadOnlyCollection<SlfStationMeasurementItem>>();
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetStationInfoAsync_StationWithLocation_ReturnsStationLocation()
        {
            // Arrange
            var slfDataService = this.CreateSlfDataServiceWithResponse(CreateStationInfoResponseJson());

            // Act
            var stationInfo = await slfDataService.GetStationInfoAsync(Network, "DAV2");

            // Assert
            stationInfo.Location.Location.Should().Be(new GeoCoordinate(46.6988884749, 9.8194108372));
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetStationInfoAsync_NearestStationWithLocation_ReturnsNearestStationLocation()
        {
            // Arrange
            var slfDataService = this.CreateSlfDataServiceWithResponse(CreateStationInfoResponseJson());

            // Act
            var stationInfo = await slfDataService.GetStationInfoAsync(Network, "DAV2");

            // Assert
            var nearestStation = stationInfo.NearestStations.Should().ContainSingle().Which;
            nearestStation.Location.Location.Should().Be(new GeoCoordinate(46.688510327, 9.8143102025));
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetStationInfoAsync_Cancelled_ThrowsOperationCanceledException()
        {
            // Arrange
            using var cancellationTokenSource = new CancellationTokenSource();
            var slfDataService = this.CreateSlfDataService(HttpMessageHandlerMocks.WaitingForCancellation().Object);

            // Act
            var stationInfoTask = slfDataService.GetStationInfoAsync(Network, "DAV2", cancellationTokenSource.Token);
            cancellationTokenSource.Cancel();
            Func<Task> action = () => stationInfoTask;

            // Assert
            await action.Should().ThrowAsync<OperationCanceledException>();
        }

        [Theory]
        [Trait(Traits.Category, Traits.UnitTests)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("..")]
        [InlineData("../DAV2")]
        [InlineData("DAV2/../..")]
        [InlineData("..\\DAV2")]
        [InlineData("DAV2?network=IMIS")]
        [InlineData("DAV2#info")]
        [InlineData("%2e%2e")]
        [InlineData(".")]
        [InlineData("DAV 2")]
        [InlineData(" DAV2")]
        public async Task GetStationInfoAsync_InvalidStationCode_ThrowsArgumentException(string stationCode)
        {
            // Arrange
            var slfDataService = this.CreateSlfDataServiceWithResponse(CreateStationInfoResponseJson());

            // Act
            Func<Task> action = () => slfDataService.GetStationInfoAsync(Network, stationCode);

            // Assert
            await action.Should().ThrowExactlyAsync<ArgumentException>();
        }

        [Theory]
        [Trait(Traits.Category, Traits.UnitTests)]
        [InlineData("")]
        [InlineData("../public")]
        public async Task GetStationInfoAsync_InvalidNetwork_ThrowsArgumentException(string network)
        {
            // Arrange
            var slfDataService = this.CreateSlfDataServiceWithResponse(CreateStationInfoResponseJson());

            // Act
            Func<Task> action = () => slfDataService.GetStationInfoAsync(network, StationCode);

            // Assert
            await action.Should().ThrowExactlyAsync<ArgumentException>();
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetStationInfoAsync_StationCodeIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            var slfDataService = this.CreateSlfDataServiceWithResponse(CreateStationInfoResponseJson());

            // Act
            Func<Task> action = () => slfDataService.GetStationInfoAsync(Network, null!);

            // Assert
            await action.Should().ThrowExactlyAsync<ArgumentNullException>();
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetLatestMeasurementByStationCodeAsync_InvalidStationCode_ThrowsArgumentException()
        {
            // Arrange
            var slfDataService = this.CreateSlfDataServiceWithLatestMeasurement(CreateLatestMeasurementResponseJson(temperatureAir: 5.316m));

            // Act
            Func<Task> action = () => slfDataService.GetLatestMeasurementByStationCodeAsync(Network, "../DAV2");

            // Assert
            await action.Should().ThrowExactlyAsync<ArgumentException>();
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetMeasurementsByStationCodeAsync_InvalidStationCode_ThrowsArgumentException()
        {
            // Arrange
            var slfDataService = this.CreateSlfDataServiceWithResponse(CreateTimeseriesResponseJson(velocityMax: 1m, velocityMean: 1m, direction: 1m));

            // Act
            Func<Task> action = () => slfDataService.GetMeasurementsByStationCodeAsync(Network, "../DAV2");

            // Assert
            await action.Should().ThrowExactlyAsync<ArgumentException>();
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetMeasurementsByStationCodeAsync_StationCodeWithAsterisk_RequestsStationCodeUnchanged()
        {
            // Arrange
            // The codes of SMN stations start with '*', for example *ABG0 (L'Abergement).
            var httpMessageHandlerMock = HttpMessageHandlerMocks.Returning(HttpStatusCode.OK, CreateSnowTimeseriesResponseJson(snowHeight: 120m, newSnowHeight: 0m, snowSurfaceTemperature: -2.5m));
            var slfDataService = this.CreateSlfDataService(httpMessageHandlerMock.Object);

            // Act
            await slfDataService.GetMeasurementsByStationCodeAsync("SMN", "*ABG0");

            // Assert
            httpMessageHandlerMock.VerifySendAsync(Times.Once(), r => r.RequestUri!.AbsolutePath == "/public/station-data/timeseries/week/current/SMN/*ABG0");
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetMapTeaserImageAsync_InvalidStationCode_ThrowsArgumentException()
        {
            // Arrange
            var slfDataService = this.CreateSlfDataServiceWithResponse("RIFF");

            // Act
            Func<Task> action = () => slfDataService.GetMapTeaserImageAsync(Network, "../DAV2");

            // Assert
            await action.Should().ThrowExactlyAsync<ArgumentException>();
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetMapTeaserImageAsync_ImageExists_ReturnsImage()
        {
            // Arrange
            var slfDataService = this.CreateSlfDataServiceWithResponse("RIFF");

            // Act
            using var imageStream = await slfDataService.GetMapTeaserImageAsync(Network, StationCode);

            // Assert
            using var streamReader = new StreamReader(imageStream);
            (await streamReader.ReadToEndAsync()).Should().Be("RIFF");
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetMapTeaserImageAsync_ImageNotFound_ThrowsMeteoSwissApiException()
        {
            // Arrange
            var slfDataService = this.CreateSlfDataService(HttpMessageHandlerMocks.Returning(HttpStatusCode.NotFound, string.Empty).Object);

            // Act
            Func<Task> action = () => slfDataService.GetMapTeaserImageAsync(Network, StationCode);

            // Assert
            var exception = (await action.Should().ThrowAsync<MeteoSwissApiException>()).Which;
            exception.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetStationInfoAsync_ServerError_ThrowsMeteoSwissApiException()
        {
            // Arrange
            var slfDataService = this.CreateSlfDataService(HttpMessageHandlerMocks.Returning(HttpStatusCode.InternalServerError, string.Empty).Object);

            // Act
            Func<Task> action = () => slfDataService.GetStationInfoAsync(Network, StationCode);

            // Assert
            var exception = (await action.Should().ThrowAsync<MeteoSwissApiException>()).Which;
            exception.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            exception.RequestUri.Should().NotBeNull();
            exception.RequestUri!.AbsolutePath.Should().Be($"/public/station-data/info/{Network}/{StationCode}");
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetStationInfoAsync_EmptyResponse_ThrowsMeteoSwissApiException()
        {
            // Arrange
            var slfDataService = this.CreateSlfDataServiceWithResponse("null");

            // Act
            Func<Task> action = () => slfDataService.GetStationInfoAsync(Network, StationCode);

            // Assert
            await action.Should().ThrowAsync<MeteoSwissApiException>();
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetStationInfoAsync_VerboseLogging_ReturnsStationInfo()
        {
            // Arrange
            // With verbose logging, the response is read as a string and logged before it is parsed.
            var options = new MeteoSwissApiOptions { VerboseLogging = true };
            var httpClient = new HttpClient(HttpMessageHandlerMocks.Returning(HttpStatusCode.OK, CreateStationInfoResponseJson()).Object);
            var slfDataService = new SlfDataService(this.logger, httpClient, options);

            // Act
            var stationInfo = await slfDataService.GetStationInfoAsync(Network, "DAV2");

            // Assert
            stationInfo.Code.Should().Be("DAV2");
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetLatestMeasurementsAsync_ServerError_ThrowsMeteoSwissApiException()
        {
            // Arrange
            var slfDataService = this.CreateSlfDataService(HttpMessageHandlerMocks.Returning(HttpStatusCode.InternalServerError, string.Empty).Object);

            // Act
            Func<Task> action = () => slfDataService.GetLatestMeasurementsAsync();

            // Assert
            var exception = (await action.Should().ThrowAsync<MeteoSwissApiException>()).Which;
            exception.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetLatestMeasurementsAsync_SnowFeatureWithWindValuesOnly_SkipsTheValue()
        {
            // Arrange
            // A feature of a snow parameter that carries wind values but no value must not fail the whole response.
            var responsesByParameter = new Dictionary<string, string>
            {
                ["SNOW_HEIGHT"] = CreateWindMeanResponseJson(velocity: 12.5m, direction: null),
            };
            var slfDataService = this.CreateSlfDataService(CreateHttpMessageHandlerMock(responsesByParameter).Object);

            // Act
            var measurements = await slfDataService.GetLatestMeasurementsAsync();

            // Assert
            measurements.Should().ContainSingle().Which.SnowHeight.Should().BeNull();
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetLatestMeasurementsAsync_StationTypeIsNotAString_ThrowsMeteoSwissApiException()
        {
            // Arrange
            var responseJson = CreateFeatureCollectionJson(CreateFeature("IMIS", "DAV2", value: 42m, timestamp: Timestamp))
                .Replace("\"type\":\"SNOW_FLAT\"", "\"type\":1");
            var responsesByParameter = new Dictionary<string, string> { ["SNOW_HEIGHT"] = responseJson };
            var slfDataService = this.CreateSlfDataService(CreateHttpMessageHandlerMock(responsesByParameter).Object);

            // Act
            Func<Task> action = () => slfDataService.GetLatestMeasurementsAsync();

            // Assert
            await action.Should().ThrowAsync<MeteoSwissApiException>();
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task Dispose_WithoutHttpClient_DisposesItsOwnClient()
        {
            // Arrange
            var slfDataService = new SlfDataService(this.logger, new MeteoSwissApiOptions());
            slfDataService.Dispose();

            // Act
            Func<Task> action = () => slfDataService.GetStationInfoAsync("IMIS", "DAV2");

            // Assert
            await action.Should().ThrowAsync<ObjectDisposedException>();
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task Dispose_WithHttpClient_DoesNotDisposeTheClient()
        {
            // Arrange
            var httpClient = new HttpClient(HttpMessageHandlerMocks.Returning(HttpStatusCode.OK, string.Empty).Object);
            var slfDataService = new SlfDataService(this.logger, httpClient, new MeteoSwissApiOptions());

            // Act
            slfDataService.Dispose();

            // Assert
            using var response = await httpClient.GetAsync(new Uri("https://localhost/"));
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public void Dispose_Twice_DoesNotThrow()
        {
            // Arrange
            var slfDataService = new SlfDataService(this.logger, new MeteoSwissApiOptions());
            slfDataService.Dispose();

            // Act
            Action action = () => slfDataService.Dispose();

            // Assert
            action.Should().NotThrow();
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public void Constructor_HttpClientIsNull_ThrowsArgumentNullException()
        {
            // Act
            Action action = () => _ = new SlfDataService(this.logger, null!, new MeteoSwissApiOptions());

            // Assert
            action.Should().Throw<ArgumentNullException>().WithParameterName("httpClient");
        }

        private SlfDataService CreateSlfDataService(HttpMessageHandler httpMessageHandler)
        {
            var httpClient = new HttpClient(httpMessageHandler);
            return new SlfDataService(this.logger, httpClient, new MeteoSwissApiOptions());
        }

        private SlfDataService CreateSlfDataServiceWithResponse(string responseJson)
        {
            return this.CreateSlfDataService(HttpMessageHandlerMocks.Returning(HttpStatusCode.OK, responseJson).Object);
        }

        /// <summary>
        /// Creates a service whose WIND_MEAN endpoint returns <paramref name="windMeanResponseJson"/>; all other endpoints return no features.
        /// </summary>
        private SlfDataService CreateSlfDataServiceWithWindMean(string windMeanResponseJson)
        {
            return this.CreateSlfDataService(new Dictionary<string, string> { ["WIND_MEAN"] = windMeanResponseJson });
        }

        /// <summary>
        /// Creates a service whose timepoint endpoints return the response of their parameter; parameters without response return no features.
        /// </summary>
        private SlfDataService CreateSlfDataService(IReadOnlyDictionary<string, string> responsesByParameter)
        {
            return this.CreateSlfDataService(CreateHttpMessageHandlerMock(responsesByParameter).Object);
        }

        private static Mock<HttpMessageHandler> CreateHttpMessageHandlerMock(IReadOnlyDictionary<string, string> responsesByParameter)
        {
            return HttpMessageHandlerMocks.Returning(request =>
            {
                var parameter = responsesByParameter.Keys.FirstOrDefault(p => request.RequestUri!.AbsolutePath.Contains($"/{p}/"));
                var responseJson = parameter != null ? responsesByParameter[parameter] : EmptyFeatureCollectionJson;
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(responseJson) };
            });
        }

        private static string CreateWindMeanResponseJson(decimal? velocity, decimal? direction)
        {
            var feature = new
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
            };

            return CreateFeatureCollectionJson(feature);
        }

        private static object CreateFeature(string network, string code, decimal? value, DateTime? timestamp)
        {
            return new
            {
                type = "Feature",
                geometry = new { type = "Point", coordinates = new[] { 9.8194108372, 46.6988884749, 2558 } },
                properties = new
                {
                    network,
                    code,
                    elevation = 2558,
                    label = "Station",
                    type = "SNOW_FLAT",
                    manual = false,
                    value,
                    timestamp,
                },
            };
        }

        private static string CreateFeatureCollectionJson(params object[] features)
        {
            return JsonSerializer.Serialize(new { type = "FeatureCollection", features });
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

        private static string CreateSnowTimeseriesResponseJson(decimal? snowHeight, decimal? newSnowHeight, decimal? snowSurfaceTemperature)
        {
            var response = new
            {
                temperatureAir = new[] { new { timestamp = Timestamp, value = 5.316m } },
                snowHeight = new[] { new { timestamp = Timestamp, value = snowHeight } },
                heightNewSnow = new[] { new { timestamp = Timestamp, value = newSnowHeight } },
                temperatureSnowSurface = new[] { new { timestamp = Timestamp, value = snowSurfaceTemperature } },
            };

            return JsonSerializer.Serialize(response);
        }

        /// <summary>
        /// Creates a service whose station info endpoint returns the station, and whose current endpoint returns <paramref name="latestMeasurementJson"/>.
        /// </summary>
        private SlfDataService CreateSlfDataServiceWithLatestMeasurement(string latestMeasurementJson)
        {
            var stationInfoJson = CreateStationInfoResponseJson();
            var httpMessageHandlerMock = HttpMessageHandlerMocks.Returning(request => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(request.RequestUri!.AbsolutePath.Contains("/info/")
                    ? stationInfoJson
                    : latestMeasurementJson),
            });

            return this.CreateSlfDataService(httpMessageHandlerMock.Object);
        }

        private static string CreateLatestMeasurementResponseJson(decimal? temperatureAir)
        {
            // Shape of the SLF current response, e.g. public/station-data/timeseries/current/IMIS/DAV2
            var response = new
            {
                temperatureAir = new { timestamp = Timestamp, value = temperatureAir },
                snowHeight = new { timestamp = Timestamp, value = 120m },
                windVelocityMean = new { timestamp = Timestamp, value = 0.796m },
                windDirectionMean = new { timestamp = Timestamp, value = 200.4m },
            };

            return JsonSerializer.Serialize(response);
        }

        private static string CreateStationInfoResponseJson()
        {
            // Shape of the SLF station info response, e.g. public/station-data/info/IMIS/DAV2
            var response = new
            {
                network = "IMIS",
                code = "DAV2",
                elevation = 2558,
                label = "Bärentälli",
                type = "SNOW_FLAT",
                manual = false,
                winterplotAvailable = true,
                location = new { type = "Point", coordinates = new[] { 9.8194108372, 46.6988884749 } },
                nearestStations = new[]
                {
                    new
                    {
                        network = "IMIS",
                        code = "DAV1",
                        elevation = 2890,
                        label = "Chrachenhorn",
                        type = "WIND",
                        manual = false,
                        distance = 1.218,
                        location = new { type = "Point", coordinates = new[] { 9.8143102025, 46.688510327 } },
                        bearing = 199,
                    },
                },
            };

            return JsonSerializer.Serialize(response);
        }
    }
}
