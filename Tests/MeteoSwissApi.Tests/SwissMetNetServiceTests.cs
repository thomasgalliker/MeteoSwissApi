using System.Collections.ObjectModel;
using System.Text;

namespace MeteoSwissApi.Tests
{
    public partial class SwissMetNetServiceTests
    {
        private const string StationsFixture = "ch.meteoschweiz.messnetz-automatisch_en.csv";
        private const string MeasurementsFixture = "VQHA80.csv";
        private const string MeasurementsFixtureDate = "202610011940";

        private static readonly TimeSpan CacheExpiration = TimeSpan.FromMinutes(20);

        private readonly ILogger<SwissMetNetService> logger;

        public SwissMetNetServiceTests(ITestOutputHelper testOutputHelper)
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

        [Theory]
        [Trait(Traits.Category, Traits.UnitTests)]
        [InlineData("de-CH")]
        [InlineData("de-DE")]
        [InlineData("fr-FR")]
        [InlineData("it-IT")]
        [InlineData("en-US")]
        public async Task GetLatestMeasurementAsync_AnyCulture_ReturnsInvariantParsedValues(string cultureName)
        {
            // Arrange
            using var cultureScope = new CultureScope(cultureName);
            var swissMetNetService = this.CreateSwissMetNetService(MeasurementsFixture);

            // Act
            var measurement = await swissMetNetService.GetLatestMeasurementAsync("TAE");

            // Assert
            measurement.Should().NotBeNull();
            measurement!.AirTemperature.Should().Be(Temperature.FromDegreesCelsius(17.4));
            measurement.RelativeAirHumidity.Should().Be(RelativeHumidity.FromPercent(81.9));
            measurement.PressureQNH.Should().Be(Pressure.FromHectopascals(1027.8));
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetLatestMeasurementAsync_MissingValue_ReturnsNull()
        {
            // Arrange
            var swissMetNetService = this.CreateSwissMetNetService(MeasurementsFixture);

            // Act
            var measurement = await swissMetNetService.GetLatestMeasurementAsync("JUN");

            // Assert
            measurement.Should().NotBeNull();
            measurement!.PressureQFF.Should().BeNull();
        }

        [Theory]
        [Trait(Traits.Category, Traits.UnitTests)]
        [InlineData("de-CH")]
        [InlineData("de-DE")]
        [InlineData("fr-FR")]
        [InlineData("it-IT")]
        [InlineData("en-US")]
        public async Task GetWeatherStationAsync_AnyCulture_ReturnsInvariantParsedLocation(string cultureName)
        {
            // Arrange
            using var cultureScope = new CultureScope(cultureName);
            var swissMetNetService = this.CreateSwissMetNetService(StationsFixture);

            // Act
            var weatherStation = await swissMetNetService.GetWeatherStationAsync("TAE");

            // Assert
            weatherStation.Should().NotBeNull();
            weatherStation!.Location.Should().Be(new GeoCoordinate(47.479892d, 8.904928d));
            weatherStation.Location!.Altitude.Should().Be(Length.FromMeters(539));
            weatherStation.BarometricAltitude.Should().Be(Length.FromMeters(539));
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetLatestMeasurementAsync_MissingPrecipitation_ReturnsNull()
        {
            // Arrange
            var swissMetNetService = this.CreateSwissMetNetService(MeasurementsFixture);

            // Act
            var measurement = await swissMetNetService.GetLatestMeasurementAsync("JUN");

            // Assert
            measurement.Should().NotBeNull();
            measurement!.Precipitation.Should().BeNull();
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetLatestMeasurementsAsync_RowWithoutDate_SkipsRow()
        {
            // Arrange
            var fixtureLines = Fixtures.GetString(MeasurementsFixture).Split('\n');
            var csvContent = string.Join("\n", fixtureLines[0], fixtureLines[1], fixtureLines[2].Replace(MeasurementsFixtureDate, "-"));
            var swissMetNetService = this.CreateSwissMetNetService(Encoding.ASCII.GetBytes(csvContent));

            // Act
            var measurements = await swissMetNetService.GetLatestMeasurementsAsync();

            // Assert
            measurements.Should().ContainSingle().Which.StationCode.Should().Be("TAE");
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetLatestMeasurementsAsync_Always_ReturnsUtcDates()
        {
            // Arrange
            var swissMetNetService = this.CreateSwissMetNetService(MeasurementsFixture);

            // Act
            var measurements = await swissMetNetService.GetLatestMeasurementsAsync();

            // Assert
            var date = measurements[0].Date;
            date.Kind.Should().Be(DateTimeKind.Utc);
            date.Should().Be(new DateTime(2026, 10, 1, 19, 40, 0, DateTimeKind.Utc));
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetLatestMeasurementAsync_DuplicateStationCode_ReturnsFirstMatch()
        {
            // Arrange
            var fixtureLines = Fixtures.GetString(MeasurementsFixture).Split('\n');
            var duplicateRow = fixtureLines[1].Replace("17.40", "99.90");
            var csvContent = string.Join("\n", fixtureLines[0], fixtureLines[1], duplicateRow);
            var swissMetNetService = this.CreateSwissMetNetService(Encoding.ASCII.GetBytes(csvContent));

            // Act
            var measurement = await swissMetNetService.GetLatestMeasurementAsync("tae");

            // Assert
            measurement.Should().NotBeNull();
            measurement!.AirTemperature.Should().Be(Temperature.FromDegreesCelsius(17.4));
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetWeatherStationsAsync_CsvWithFooter_ReturnsOnlyStations()
        {
            // Arrange
            var swissMetNetService = this.CreateSwissMetNetService(StationsFixture);

            // Act
            var weatherStations = await swissMetNetService.GetWeatherStationsAsync();

            // Assert
            weatherStations.Select(s => s.StationCode).Should().Equal("TAE", "ABE", "CHZ", "VSCHY", "GAD", "JUN", "KLO");
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetWeatherStationAsync_SeveralDataOwners_ReturnsTrimmedDataOwners()
        {
            // Arrange
            var swissMetNetService = this.CreateSwissMetNetService(StationsFixture);

            // Act
            var weatherStation = await swissMetNetService.GetWeatherStationAsync("GAD");

            // Assert
            weatherStation.Should().NotBeNull();
            weatherStation!.DataOwners.Should().Equal("Kt. BE", "SLF");
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetWeatherStationAsync_PlaceWithUmlaut_ReturnsDecodedPlace()
        {
            // Arrange
            // The stations file is encoded in Windows-1252.
            var swissMetNetService = this.CreateSwissMetNetService(StationsFixture);

            // Act
            var weatherStation = await swissMetNetService.GetWeatherStationAsync("TAE");

            // Assert
            weatherStation.Should().NotBeNull();
            weatherStation!.Place.Should().Be("Aadorf / Tänikon");
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetWeatherStationAsync_UnknownStationCode_ReturnsNull()
        {
            // Arrange
            var swissMetNetService = this.CreateSwissMetNetService(StationsFixture);

            // Act
            var weatherStation = await swissMetNetService.GetWeatherStationAsync("XXX");

            // Assert
            weatherStation.Should().BeNull();
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetWeatherStationsAsync_ServerError_ThrowsMeteoSwissApiException()
        {
            // Arrange
            var swissMetNetService = this.CreateSwissMetNetService(HttpStatusCode.InternalServerError, Array.Empty<byte>());

            // Act
            Func<Task> action = () => swissMetNetService.GetWeatherStationsAsync();

            // Assert
            var exception = (await action.Should().ThrowAsync<MeteoSwissApiException>()).Which;
            exception.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            exception.RequestUri.AbsolutePath.Should().EndWith(StationsFixture);
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetLatestMeasurementsAsync_EmptyResponse_ThrowsMeteoSwissApiException()
        {
            // Arrange
            var swissMetNetService = this.CreateSwissMetNetService(Array.Empty<byte>());

            // Act
            Func<Task> action = () => swissMetNetService.GetLatestMeasurementsAsync();

            // Assert
            var exception = (await action.Should().ThrowAsync<MeteoSwissApiException>()).Which;
            exception.RequestUri.AbsolutePath.Should().EndWith(MeasurementsFixture);
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetWeatherStationsAsync_Cancelled_ThrowsOperationCanceledException()
        {
            // Arrange
            using var cancellationTokenSource = new CancellationTokenSource();
            var swissMetNetService = this.CreateSwissMetNetService(HttpMessageHandlerMocks.WaitingForCancellation().Object);

            // Act
            var weatherStationsTask = swissMetNetService.GetWeatherStationsAsync(cancellationTokenSource.Token);
            cancellationTokenSource.Cancel();
            Func<Task> action = () => weatherStationsTask;

            // Assert
            await action.Should().ThrowAsync<OperationCanceledException>();
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetWeatherStationsAsync_Always_SendsNoCacheHeader()
        {
            // Arrange
            var httpMessageHandlerMock = HttpMessageHandlerMocks.Returning(HttpStatusCode.OK, Fixtures.GetBytes(StationsFixture));
            var swissMetNetService = this.CreateSwissMetNetService(httpMessageHandlerMock.Object);

            // Act
            await swissMetNetService.GetWeatherStationsAsync();

            // Assert
            httpMessageHandlerMock.VerifySendAsync(Times.Once(), r => r.Headers.CacheControl.NoCache);
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetWeatherStationsAsync_Always_ReturnsReadOnlyList()
        {
            // Arrange
            var swissMetNetService = this.CreateSwissMetNetService(StationsFixture);

            // Act
            var weatherStations = await swissMetNetService.GetWeatherStationsAsync();

            // Assert
            weatherStations.Should().BeOfType<ReadOnlyCollection<WeatherStation>>();
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetWeatherStationsAsync_CacheExpiration_DownloadsOnce()
        {
            // Arrange
            var httpMessageHandlerMock = HttpMessageHandlerMocks.Returning(HttpStatusCode.OK, Fixtures.GetBytes(StationsFixture));
            var swissMetNetService = this.CreateSwissMetNetService(httpMessageHandlerMock.Object);

            // Act
            var firstWeatherStations = await swissMetNetService.GetWeatherStationsAsync(TimeSpan.FromDays(30));
            var secondWeatherStations = await swissMetNetService.GetWeatherStationsAsync(TimeSpan.FromDays(30));

            // Assert
            secondWeatherStations.Should().BeSameAs(firstWeatherStations);
            httpMessageHandlerMock.VerifySendAsync(Times.Once());
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetWeatherStationsAsync_NoCacheExpiration_DownloadsEveryTime()
        {
            // Arrange
            var httpMessageHandlerMock = HttpMessageHandlerMocks.Returning(HttpStatusCode.OK, Fixtures.GetBytes(StationsFixture));
            var swissMetNetService = this.CreateSwissMetNetService(httpMessageHandlerMock.Object);

            // Act
            await swissMetNetService.GetWeatherStationsAsync();
            await swissMetNetService.GetWeatherStationsAsync();

            // Assert
            httpMessageHandlerMock.VerifySendAsync(Times.Exactly(2));
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetWeatherStationsAsync_ConcurrentCalls_DownloadsOnce()
        {
            // Arrange
            // The response of the first request is held back until the test releases it, so both calls miss the cache.
            var responseReleased = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            var requestCount = 0;
            var httpMessageHandlerMock = HttpMessageHandlerMocks.Returning(async (HttpRequestMessage _, CancellationToken _) =>
            {
                Interlocked.Increment(ref requestCount);
                await responseReleased.Task;
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new ByteArrayContent(Fixtures.GetBytes(StationsFixture)) };
            });
            var swissMetNetService = this.CreateSwissMetNetService(httpMessageHandlerMock.Object);

            // Act
            var firstCall = swissMetNetService.GetWeatherStationsAsync(TimeSpan.FromDays(30));
            var secondCall = swissMetNetService.GetWeatherStationsAsync(TimeSpan.FromDays(30));
            responseReleased.SetResult(true);
            var weatherStations = await Task.WhenAll(firstCall, secondCall);

            // Assert
            requestCount.Should().Be(1);
            weatherStations[1].Should().BeSameAs(weatherStations[0]);
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetLatestMeasurementsAsync_RecentMeasurementsAndCacheExpiration_DownloadsOnce()
        {
            // Arrange
            var httpMessageHandlerMock = HttpMessageHandlerMocks.Returning(HttpStatusCode.OK, CreateMeasurementsCsvDatedNow());
            var swissMetNetService = this.CreateSwissMetNetService(httpMessageHandlerMock.Object);

            // Act
            var firstMeasurements = await swissMetNetService.GetLatestMeasurementsAsync(CacheExpiration);
            var secondMeasurements = await swissMetNetService.GetLatestMeasurementsAsync(CacheExpiration);

            // Assert
            secondMeasurements.Should().BeSameAs(firstMeasurements);
            httpMessageHandlerMock.VerifySendAsync(Times.Once());
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetLatestMeasurementsAsync_CacheExpirationFromOptions_DownloadsOnce()
        {
            // Arrange
            var options = new MeteoSwissApiOptions { SwissMetNet = { CacheExpiration = CacheExpiration } };
            var httpMessageHandlerMock = HttpMessageHandlerMocks.Returning(HttpStatusCode.OK, CreateMeasurementsCsvDatedNow());
            var swissMetNetService = this.CreateSwissMetNetService(httpMessageHandlerMock.Object, options);

            // Act
            await swissMetNetService.GetLatestMeasurementsAsync();
            await swissMetNetService.GetLatestMeasurementsAsync();

            // Assert
            httpMessageHandlerMock.VerifySendAsync(Times.Once());
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetLatestMeasurementsAsync_MeasurementsOlderThanCacheExpiration_DownloadsEveryTime()
        {
            // Arrange
            // The measurements of the fixture are dated 2026-10-01, so they are older than the cache expiration:
            // the cache would hide newer values, and every call downloads until MeteoSwiss publishes them.
            var httpMessageHandlerMock = HttpMessageHandlerMocks.Returning(HttpStatusCode.OK, Fixtures.GetBytes(MeasurementsFixture));
            var swissMetNetService = this.CreateSwissMetNetService(httpMessageHandlerMock.Object);

            // Act
            await swissMetNetService.GetLatestMeasurementsAsync(CacheExpiration);
            await swissMetNetService.GetLatestMeasurementsAsync(CacheExpiration);

            // Assert
            httpMessageHandlerMock.VerifySendAsync(Times.Exactly(2));
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetLatestMeasurementsAsync_NoCacheExpiration_DownloadsEveryTime()
        {
            // Arrange
            var httpMessageHandlerMock = HttpMessageHandlerMocks.Returning(HttpStatusCode.OK, CreateMeasurementsCsvDatedNow());
            var swissMetNetService = this.CreateSwissMetNetService(httpMessageHandlerMock.Object);

            // Act
            await swissMetNetService.GetLatestMeasurementsAsync();
            await swissMetNetService.GetLatestMeasurementsAsync();

            // Assert
            httpMessageHandlerMock.VerifySendAsync(Times.Exactly(2));
        }

        [Theory]
        [Trait(Traits.Category, Traits.UnitTests)]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task GetLatestMeasurementsAsync_NonPositiveCacheExpiration_ThrowsArgumentOutOfRangeException(int cacheExpirationMinutes)
        {
            // Arrange
            var swissMetNetService = this.CreateSwissMetNetService(MeasurementsFixture);

            // Act
            Func<Task> action = () => swissMetNetService.GetLatestMeasurementsAsync(TimeSpan.FromMinutes(cacheExpirationMinutes));

            // Assert
            await action.Should().ThrowAsync<ArgumentOutOfRangeException>().WithParameterName("cacheExpiration");
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetLatestMeasurementsAsync_Always_ReturnsReadOnlyList()
        {
            // Arrange
            var swissMetNetService = this.CreateSwissMetNetService(MeasurementsFixture);

            // Act
            var measurements = await swissMetNetService.GetLatestMeasurementsAsync();

            // Assert
            measurements.Should().BeOfType<ReadOnlyCollection<WeatherStationMeasurement>>();
        }

        [Theory]
        [Trait(Traits.Category, Traits.UnitTests)]
        [InlineData(0)]
        [InlineData(-1)]
        public void Constructor_NonPositiveCacheExpirationInOptions_ThrowsArgumentException(int cacheExpirationMinutes)
        {
            // Arrange
            var options = new MeteoSwissApiOptions { SwissMetNet = { CacheExpiration = TimeSpan.FromMinutes(cacheExpirationMinutes) } };

            // Act
            Action action = () => _ = new SwissMetNetService(this.logger, options);

            // Assert
            action.Should().Throw<ArgumentException>().WithParameterName("options");
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public void Constructor_SwissMetNetOptionsAreNull_ThrowsArgumentException()
        {
            // Arrange
            var options = new MeteoSwissApiOptions { SwissMetNet = null };

            // Act
            Action action = () => _ = new SwissMetNetService(this.logger, options);

            // Assert
            action.Should().Throw<ArgumentException>().WithParameterName("options");
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetWeatherStationsAsync_AfterDispose_ThrowsObjectDisposedException()
        {
            // Arrange
            var swissMetNetService = this.CreateSwissMetNetService(StationsFixture);
            swissMetNetService.Dispose();

            // Act
            Func<Task> action = () => swissMetNetService.GetWeatherStationsAsync();

            // Assert
            await action.Should().ThrowAsync<ObjectDisposedException>();
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public void Dispose_Twice_DoesNotThrow()
        {
            // Arrange
            var swissMetNetService = this.CreateSwissMetNetService(StationsFixture);
            swissMetNetService.Dispose();

            // Act
            Action action = () => swissMetNetService.Dispose();

            // Assert
            action.Should().NotThrow();
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task Dispose_WithHttpClient_DoesNotDisposeTheClient()
        {
            // Arrange
            var httpClient = new HttpClient(HttpMessageHandlerMocks.Returning(HttpStatusCode.OK, string.Empty).Object);
            var swissMetNetService = new SwissMetNetService(this.logger, httpClient, new MeteoSwissApiOptions());

            // Act
            swissMetNetService.Dispose();

            // Assert
            using var response = await httpClient.GetAsync(new Uri("https://localhost/"));
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public void Constructor_HttpClientIsNull_ThrowsArgumentNullException()
        {
            // Act
            Action action = () => _ = new SwissMetNetService(this.logger, (HttpClient)null, new MeteoSwissApiOptions());

            // Assert
            action.Should().Throw<ArgumentNullException>().WithParameterName("httpClient");
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetWeatherStationsAsync_CacheControlChangedByAHandler_SendsAFreshHeaderWithTheNextRequest()
        {
            // Arrange
            // A message handler of the caller changes the Cache-Control header of a request; the next request must not carry that change.
            var noStoreValues = new List<bool?>();
            var httpMessageHandlerMock = HttpMessageHandlerMocks.Returning(request =>
            {
                var cacheControl = request.Headers.CacheControl;
                noStoreValues.Add(cacheControl?.NoStore);
                cacheControl!.NoStore = true;
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new ByteArrayContent(Fixtures.GetBytes(StationsFixture)) };
            });
            var swissMetNetService = this.CreateSwissMetNetService(httpMessageHandlerMock.Object);

            // Act
            await swissMetNetService.GetWeatherStationsAsync();
            await swissMetNetService.GetWeatherStationsAsync();

            // Assert
            noStoreValues.Should().Equal(false, false);
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetWeatherStationsAsync_MaxValueCacheExpiration_CachesTheStations()
        {
            // Arrange
            var httpMessageHandlerMock = HttpMessageHandlerMocks.Returning(HttpStatusCode.OK, Fixtures.GetBytes(StationsFixture));
            var swissMetNetService = this.CreateSwissMetNetService(httpMessageHandlerMock.Object);

            // Act
            await swissMetNetService.GetWeatherStationsAsync(TimeSpan.MaxValue);
            await swissMetNetService.GetWeatherStationsAsync(TimeSpan.MaxValue);

            // Assert
            httpMessageHandlerMock.VerifySendAsync(Times.Once());
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetLatestMeasurementsAsync_MaxValueCacheExpiration_CachesTheMeasurements()
        {
            // Arrange
            var httpMessageHandlerMock = HttpMessageHandlerMocks.Returning(HttpStatusCode.OK, CreateMeasurementsCsvDatedNow());
            var swissMetNetService = this.CreateSwissMetNetService(httpMessageHandlerMock.Object);

            // Act
            await swissMetNetService.GetLatestMeasurementsAsync(TimeSpan.MaxValue);
            await swissMetNetService.GetLatestMeasurementsAsync(TimeSpan.MaxValue);

            // Assert
            httpMessageHandlerMock.VerifySendAsync(Times.Once());
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetWeatherStationsAsync_OptionsChangedAfterConstruction_KeepsTheConstructionTimeOptions()
        {
            // Arrange
            var options = new MeteoSwissApiOptions();
            var httpMessageHandlerMock = HttpMessageHandlerMocks.Returning(HttpStatusCode.OK, Fixtures.GetBytes(StationsFixture));
            var swissMetNetService = this.CreateSwissMetNetService(httpMessageHandlerMock.Object, options);
            options.SwissMetNet.CacheExpiration = CacheExpiration;

            // Act
            await swissMetNetService.GetWeatherStationsAsync();
            await swissMetNetService.GetWeatherStationsAsync();

            // Assert
            httpMessageHandlerMock.VerifySendAsync(Times.Exactly(2));
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetLatestMeasurementsAsync_RowWithoutDate_LogsTheSkippedRows()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<SwissMetNetService>>();
            var fixtureLines = Fixtures.GetString(MeasurementsFixture).Split('\n');
            var csvContent = string.Join("\n", fixtureLines[0], fixtureLines[1], fixtureLines[2].Replace(MeasurementsFixtureDate, "-"));
            var httpClient = new HttpClient(HttpMessageHandlerMocks.Returning(HttpStatusCode.OK, Encoding.ASCII.GetBytes(csvContent)).Object);
            var swissMetNetService = new SwissMetNetService(loggerMock.Object, httpClient, new MeteoSwissApiOptions());

            // Act
            await swissMetNetService.GetLatestMeasurementsAsync();

            // Assert
            loggerMock.Verify(
                l => l.Log(
                    LogLevel.Debug,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((state, _) => state.ToString().Contains("skipped 1 rows without a readable date")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetWeatherStationAsync_StationCodeIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            var swissMetNetService = this.CreateSwissMetNetService(StationsFixture);

            // Act
            Func<Task> action = () => swissMetNetService.GetWeatherStationAsync(null);

            // Assert
            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetLatestMeasurementAsync_StationCodeIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            var swissMetNetService = this.CreateSwissMetNetService(MeasurementsFixture);

            // Act
            Func<Task> action = () => swissMetNetService.GetLatestMeasurementAsync(null);

            // Assert
            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        /// <summary>
        /// The measurements of the fixture, dated now instead of 2026-10-01, so that they are younger than the cache expiration.
        /// </summary>
        private static byte[] CreateMeasurementsCsvDatedNow()
        {
            var currentDate = DateTime.UtcNow.ToString("yyyyMMddHHmm", CultureInfo.InvariantCulture);
            return Encoding.ASCII.GetBytes(Fixtures.GetString(MeasurementsFixture).Replace(MeasurementsFixtureDate, currentDate));
        }

        private SwissMetNetService CreateSwissMetNetService(string fixtureFileName)
        {
            return this.CreateSwissMetNetService(Fixtures.GetBytes(fixtureFileName));
        }

        private SwissMetNetService CreateSwissMetNetService(byte[] csvContent)
        {
            return this.CreateSwissMetNetService(HttpStatusCode.OK, csvContent);
        }

        private SwissMetNetService CreateSwissMetNetService(HttpStatusCode statusCode, byte[] csvContent)
        {
            return this.CreateSwissMetNetService(HttpMessageHandlerMocks.Returning(statusCode, csvContent).Object);
        }

        private SwissMetNetService CreateSwissMetNetService(HttpMessageHandler httpMessageHandler)
        {
            return this.CreateSwissMetNetService(httpMessageHandler, new MeteoSwissApiOptions());
        }

        private SwissMetNetService CreateSwissMetNetService(HttpMessageHandler httpMessageHandler, MeteoSwissApiOptions options)
        {
            var httpClient = new HttpClient(httpMessageHandler);
            return new SwissMetNetService(this.logger, httpClient, options);
        }
    }
}