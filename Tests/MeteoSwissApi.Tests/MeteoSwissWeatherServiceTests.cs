using System.Text.Json;
using Microsoft.Extensions.Options;

namespace MeteoSwissApi.Tests
{
    public partial class MeteoSwissWeatherServiceTests
    {
        private const string PlzDetailFixture = "plzDetail_v3_633000.json";
        private const string ForecastFixture = "forecast_v3_633000.json";
        private const int Plz = 6330;

        private readonly ILogger<MeteoSwissWeatherService> logger;

        public MeteoSwissWeatherServiceTests(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
            this.logger = new TestOutputHelperLogger<MeteoSwissWeatherService>(testOutputHelper);
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

            this.testHelper = new TestHelper(testOutputHelper);
        }

        [Theory]
        [Trait(Traits.Category, Traits.UnitTests)]
        [InlineData(1000, "100000")]
        [InlineData(8001, "800100")]
        [InlineData(9999, "999900")]
        [InlineData(100000, "100000")]
        [InlineData(601002, "601002")]
        [InlineData(999999, "999999")]
        public async Task GetCurrentWeatherAsync_ValidPlz_RequestsPlzWithSuffix(int plz, string expectedPlz)
        {
            // Arrange
            var expectedQuery = $"?plz={expectedPlz}";
            var httpMessageHandlerMock = HttpMessageHandlerMocks.Returning(HttpStatusCode.OK, Fixtures.GetString(PlzDetailFixture));
            var meteoSwissWeatherService = this.CreateMeteoSwissWeatherService(httpMessageHandlerMock.Object);

            // Act
            await meteoSwissWeatherService.GetCurrentWeatherAsync(plz);

            // Assert
            httpMessageHandlerMock.VerifySendAsync(Times.Once(), r => r.RequestUri.Query == expectedQuery);
        }

        [Theory]
        [Trait(Traits.Category, Traits.UnitTests)]
        [InlineData(-100)]
        [InlineData(0)]
        [InlineData(999)]
        [InlineData(10000)]
        [InlineData(60100)]
        [InlineData(99999)]
        [InlineData(1000000)]
        public async Task GetCurrentWeatherAsync_InvalidPlz_ThrowsArgumentOutOfRangeException(int plz)
        {
            // Arrange
            var meteoSwissWeatherService = this.CreateMeteoSwissWeatherService(HttpStatusCode.OK, Fixtures.GetString(PlzDetailFixture));

            // Act
            Func<Task> action = () => meteoSwissWeatherService.GetCurrentWeatherAsync(plz);

            // Assert
            await action.Should().ThrowAsync<ArgumentOutOfRangeException>();
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetCurrentWeatherAsync_ValidResponse_ReturnsWeatherInfo()
        {
            // Arrange
            var meteoSwissWeatherService = this.CreateMeteoSwissWeatherService(HttpStatusCode.OK, Fixtures.GetString(PlzDetailFixture));

            // Act
            var weatherInfo = await meteoSwissWeatherService.GetCurrentWeatherAsync(Plz);

            // Assert
            weatherInfo.CurrentWeather.Temperature.Should().Be(Temperature.FromDegreesCelsius(25.3));
            weatherInfo.Forecast.Should().NotBeEmpty();
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetCurrentWeatherAsync_VerboseLogging_ReturnsWeatherInfo()
        {
            // Arrange
            // With verbose logging, the response is read as a string and logged before it is parsed.
            var options = new MeteoSwissApiOptions { VerboseLogging = true };
            var meteoSwissWeatherService = this.CreateMeteoSwissWeatherService(HttpMessageHandlerMocks.Returning(HttpStatusCode.OK, Fixtures.GetString(PlzDetailFixture)).Object, options);

            // Act
            var weatherInfo = await meteoSwissWeatherService.GetCurrentWeatherAsync(Plz);

            // Assert
            weatherInfo.CurrentWeather.Temperature.Should().Be(Temperature.FromDegreesCelsius(25.3));
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetCurrentWeatherAsync_Language_SendsAcceptLanguageHeader()
        {
            // Arrange
            var options = new MeteoSwissApiOptions { Language = "de" };
            var httpMessageHandlerMock = HttpMessageHandlerMocks.Returning(HttpStatusCode.OK, Fixtures.GetString(PlzDetailFixture));
            var meteoSwissWeatherService = this.CreateMeteoSwissWeatherService(httpMessageHandlerMock.Object, options);

            // Act
            await meteoSwissWeatherService.GetCurrentWeatherAsync(Plz);

            // Assert
            httpMessageHandlerMock.VerifySendAsync(Times.Once(), r => r.Headers.AcceptLanguage.Any(l => l.Value == "de"));
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetCurrentWeatherAsync_Cancelled_ThrowsOperationCanceledException()
        {
            // Arrange
            using var cancellationTokenSource = new CancellationTokenSource();
            var meteoSwissWeatherService = this.CreateMeteoSwissWeatherService(HttpMessageHandlerMocks.WaitingForCancellation().Object);

            // Act
            var weatherInfoTask = meteoSwissWeatherService.GetCurrentWeatherAsync(Plz, cancellationTokenSource.Token);
            cancellationTokenSource.Cancel();
            Func<Task> action = () => weatherInfoTask;

            // Assert
            await action.Should().ThrowAsync<OperationCanceledException>();
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetCurrentWeatherAsync_ServerError_ThrowsMeteoSwissApiException()
        {
            // Arrange
            var meteoSwissWeatherService = this.CreateMeteoSwissWeatherService(HttpStatusCode.InternalServerError, string.Empty);

            // Act
            Func<Task> action = () => meteoSwissWeatherService.GetCurrentWeatherAsync(Plz);

            // Assert
            var exception = (await action.Should().ThrowAsync<MeteoSwissApiException>()).Which;
            exception.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            exception.RequestUri.AbsolutePath.Should().Be("/v3/plzDetail");
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetCurrentWeatherAsync_NetworkError_ThrowsMeteoSwissApiException()
        {
            // Arrange
            var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new HttpRequestException("No such host is known."));

            var meteoSwissWeatherService = this.CreateMeteoSwissWeatherService(httpMessageHandlerMock.Object);

            // Act
            Func<Task> action = () => meteoSwissWeatherService.GetCurrentWeatherAsync(Plz);

            // Assert
            var exception = (await action.Should().ThrowAsync<MeteoSwissApiException>()).Which;
            exception.InnerException.Should().BeOfType<HttpRequestException>();
        }

        [Theory]
        [Trait(Traits.Category, Traits.UnitTests)]
        [InlineData("null")]
        [InlineData("")]
        public async Task GetCurrentWeatherAsync_EmptyResponse_ThrowsMeteoSwissApiException(string responseJson)
        {
            // Arrange
            var meteoSwissWeatherService = this.CreateMeteoSwissWeatherService(HttpStatusCode.OK, responseJson);

            // Act
            Func<Task> action = () => meteoSwissWeatherService.GetCurrentWeatherAsync(Plz);

            // Assert
            await action.Should().ThrowAsync<MeteoSwissApiException>();
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetCurrentWeatherAsync_InvalidJson_ThrowsMeteoSwissApiException()
        {
            // Arrange
            var meteoSwissWeatherService = this.CreateMeteoSwissWeatherService(HttpStatusCode.OK, "<html>Service unavailable</html>");

            // Act
            Func<Task> action = () => meteoSwissWeatherService.GetCurrentWeatherAsync(Plz);

            // Assert
            var exception = (await action.Should().ThrowAsync<MeteoSwissApiException>()).Which;
            exception.InnerException.Should().BeAssignableTo<JsonException>();
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetCurrentWeatherAsync_UnmappedJsonMember_ThrowsMeteoSwissApiException_IfDisallowed()
        {
            // Arrange
            // A property MeteoSwiss adds to the API fails the strict check the integration tests use.
            var responseJson = Fixtures.GetString(PlzDetailFixture).Insert(1, "\"addedByMeteoSwiss\":1,");
            var meteoSwissWeatherService = this.CreateMeteoSwissWeatherService(HttpStatusCode.OK, responseJson);
            meteoSwissWeatherService.DisallowUnmappedJsonMembers = true;

            // Act
            Func<Task> action = () => meteoSwissWeatherService.GetCurrentWeatherAsync(Plz);

            // Assert
            var exception = (await action.Should().ThrowAsync<MeteoSwissApiException>()).Which;
            exception.InnerException.Should().BeAssignableTo<JsonException>();
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetCurrentWeatherAsync_UnmappedJsonMember_ReturnsWeatherInfo_IfAllowed()
        {
            // Arrange
            var responseJson = Fixtures.GetString(PlzDetailFixture).Insert(1, "\"addedByMeteoSwiss\":1,");
            var meteoSwissWeatherService = this.CreateMeteoSwissWeatherService(HttpStatusCode.OK, responseJson);

            // Act
            var weatherInfo = await meteoSwissWeatherService.GetCurrentWeatherAsync(Plz);

            // Assert
            weatherInfo.CurrentWeather.Should().NotBeNull();
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetCurrentWeatherAsync_UnknownWarnType_LogsWarnType()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<MeteoSwissWeatherService>>();
            var responseJson = Fixtures.GetString(PlzDetailFixture)
                .Replace("\"warnType\":10,\"warnLevel\":4", "\"warnType\":99,\"warnLevel\":4");
            var httpClient = new HttpClient(HttpMessageHandlerMocks.Returning(HttpStatusCode.OK, responseJson).Object);
            var meteoSwissWeatherService = new MeteoSwissWeatherService(loggerMock.Object, httpClient, new MeteoSwissApiOptions());

            // Act
            await meteoSwissWeatherService.GetCurrentWeatherAsync(Plz);

            // Assert
            loggerMock.Verify(
                l => l.Log(
                    LogLevel.Debug,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((state, _) => state.ToString().Contains("warnType=99")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetForecastAsync_ValidPlz_RequestsPlzWithSuffix()
        {
            // Arrange
            var httpMessageHandlerMock = HttpMessageHandlerMocks.Returning(HttpStatusCode.OK, Fixtures.GetString(ForecastFixture));
            var meteoSwissWeatherService = this.CreateMeteoSwissWeatherService(httpMessageHandlerMock.Object);

            // Act
            await meteoSwissWeatherService.GetForecastAsync(8001);

            // Assert
            httpMessageHandlerMock.VerifySendAsync(Times.Once(), r => r.RequestUri.Query == "?plz=800100");
        }

        [Theory]
        [Trait(Traits.Category, Traits.UnitTests)]
        [InlineData(-100)]
        [InlineData(60100)]
        [InlineData(1000000)]
        public async Task GetForecastAsync_InvalidPlz_ThrowsArgumentOutOfRangeException(int plz)
        {
            // Arrange
            var meteoSwissWeatherService = this.CreateMeteoSwissWeatherService(HttpStatusCode.OK, Fixtures.GetString(ForecastFixture));

            // Act
            Func<Task> action = () => meteoSwissWeatherService.GetForecastAsync(plz);

            // Assert
            await action.Should().ThrowAsync<ArgumentOutOfRangeException>();
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetForecastAsync_ServerError_ThrowsMeteoSwissApiException()
        {
            // Arrange
            var meteoSwissWeatherService = this.CreateMeteoSwissWeatherService(HttpStatusCode.ServiceUnavailable, string.Empty);

            // Act
            Func<Task> action = () => meteoSwissWeatherService.GetForecastAsync(Plz);

            // Assert
            var exception = (await action.Should().ThrowAsync<MeteoSwissApiException>()).Which;
            exception.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);
            exception.RequestUri.AbsolutePath.Should().Be("/v3/forecast");
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetWeatherIconAsync_DefaultMapping_DownloadsIcon()
        {
            // Arrange
            var httpMessageHandlerMock = HttpMessageHandlerMocks.Returning(HttpStatusCode.OK, "<svg>1</svg>");
            var meteoSwissWeatherService = this.CreateMeteoSwissWeatherService(httpMessageHandlerMock.Object);

            // Act
            using var iconStream = await meteoSwissWeatherService.GetWeatherIconAsync(1);

            // Assert
            using var streamReader = new StreamReader(iconStream);
            (await streamReader.ReadToEndAsync()).Should().Be("<svg>1</svg>");
            httpMessageHandlerMock.VerifySendAsync(Times.Once(), r => r.RequestUri.AbsolutePath.EndsWith("/weather-symbols/1.svg"));
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetWeatherIconAsync_MappingIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            var meteoSwissWeatherService = this.CreateMeteoSwissWeatherService(HttpStatusCode.OK, string.Empty);

            // Act
            Func<Task> action = () => meteoSwissWeatherService.GetWeatherIconAsync(1, weatherIconMapping: null);

            // Assert
            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetWarningIconAsync_MappingIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            var meteoSwissWeatherService = this.CreateMeteoSwissWeatherService(HttpStatusCode.OK, string.Empty);

            // Act
            Func<Task> action = () => meteoSwissWeatherService.GetWarningIconAsync(WarnLevel.Level3, warningIconMapping: null);

            // Assert
            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Theory]
        [Trait(Traits.Category, Traits.UnitTests)]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("es")]
        [InlineData("de-CH")]
        public void Constructor_UnsupportedLanguage_ThrowsArgumentException(string language)
        {
            // Arrange
            var options = new MeteoSwissApiOptions { Language = language };

            // Act
            Action action = () => _ = new MeteoSwissWeatherService(this.logger, options);

            // Assert
            action.Should().Throw<ArgumentException>().WithParameterName("options");
        }

        [Theory]
        [Trait(Traits.Category, Traits.UnitTests)]
        [InlineData("de")]
        [InlineData("FR")]
        public void Constructor_SupportedLanguage_CreatesService(string language)
        {
            // Arrange
            var options = new MeteoSwissApiOptions { Language = language };

            // Act
            var meteoSwissWeatherService = new MeteoSwissWeatherService(this.logger, options);

            // Assert
            meteoSwissWeatherService.Should().NotBeNull();
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public void Constructor_LoggerIsNull_ThrowsArgumentNullException()
        {
            // Act
            Action action = () => _ = new MeteoSwissWeatherService(logger: null, new MeteoSwissApiOptions());

            // Assert
            action.Should().Throw<ArgumentNullException>().WithParameterName("logger");
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetForecastAsync_UnknownPlz_ThrowsMeteoSwissApiException()
        {
            // Arrange
            // MeteoSwiss answers an unknown postal code with status 200 and a placeholder whose plz is 0.
            var meteoSwissWeatherService = this.CreateMeteoSwissWeatherService(HttpStatusCode.OK, Fixtures.GetString("forecast_v3_999900_unknown.json"));

            // Act
            Func<Task> action = () => meteoSwissWeatherService.GetForecastAsync(999900);

            // Assert
            var exception = (await action.Should().ThrowAsync<MeteoSwissApiException>()).Which;
            exception.Message.Should().Be("MeteoSwiss does not know the PLZ 999900.");
            exception.RequestUri.AbsolutePath.Should().Be("/v3/forecast");
            exception.StatusCode.Should().BeNull();
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetForecastAsync_KnownPlz_ReturnsForecastInfo()
        {
            // Arrange
            var meteoSwissWeatherService = this.CreateMeteoSwissWeatherService(HttpStatusCode.OK, Fixtures.GetString(ForecastFixture));

            // Act
            var forecastInfo = await meteoSwissWeatherService.GetForecastAsync(Plz);

            // Assert
            forecastInfo.Plz.Should().Be(633000);
            forecastInfo.CurrentWeather.IconV2.Should().Be(2);
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public void Constructor_WithHttpClient_DoesNotChangeItsDefaultRequestHeaders()
        {
            // Arrange
            var httpClient = new HttpClient(HttpMessageHandlerMocks.Returning(HttpStatusCode.OK, string.Empty).Object);

            // Act
            _ = new MeteoSwissWeatherService(this.logger, httpClient, new MeteoSwissApiOptions { Language = "de" });
            _ = new SwissMetNetService(new TestOutputHelperLogger<SwissMetNetService>(this.testOutputHelper), httpClient, new MeteoSwissApiOptions());

            // Assert
            httpClient.DefaultRequestHeaders.Should().BeEmpty();
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetCurrentWeatherAsync_TwoServicesOnOneHttpClient_SendsOneAcceptLanguageValue()
        {
            // Arrange
            var httpMessageHandlerMock = HttpMessageHandlerMocks.Returning(HttpStatusCode.OK, Fixtures.GetString(PlzDetailFixture));
            var httpClient = new HttpClient(httpMessageHandlerMock.Object);
            var firstService = new MeteoSwissWeatherService(this.logger, httpClient, new MeteoSwissApiOptions { Language = "de" });
            _ = new MeteoSwissWeatherService(this.logger, httpClient, new MeteoSwissApiOptions { Language = "fr" });

            // Act
            await firstService.GetCurrentWeatherAsync(Plz);

            // Assert
            httpMessageHandlerMock.VerifySendAsync(Times.Once(), r => r.Headers.AcceptLanguage.Count == 1 && r.Headers.AcceptLanguage.Single().Value == "de");
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task Dispose_WithoutHttpClient_DisposesItsOwnClient()
        {
            // Arrange
            var meteoSwissWeatherService = new MeteoSwissWeatherService(this.logger, new MeteoSwissApiOptions());
            meteoSwissWeatherService.Dispose();

            // Act
            Func<Task> action = () => meteoSwissWeatherService.GetForecastAsync(Plz);

            // Assert
            await action.Should().ThrowAsync<ObjectDisposedException>();
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task Dispose_WithHttpClient_DoesNotDisposeTheClient()
        {
            // Arrange
            var httpClient = new HttpClient(HttpMessageHandlerMocks.Returning(HttpStatusCode.OK, string.Empty).Object);
            var meteoSwissWeatherService = new MeteoSwissWeatherService(this.logger, httpClient, new MeteoSwissApiOptions());

            // Act
            meteoSwissWeatherService.Dispose();

            // Assert
            using var response = await httpClient.GetAsync(new Uri("https://localhost/"));
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public void Dispose_Twice_DoesNotThrow()
        {
            // Arrange
            var meteoSwissWeatherService = new MeteoSwissWeatherService(this.logger, new MeteoSwissApiOptions());
            meteoSwissWeatherService.Dispose();

            // Act
            Action action = () => meteoSwissWeatherService.Dispose();

            // Assert
            action.Should().NotThrow();
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public void Constructor_HttpClientIsNull_ThrowsArgumentNullException()
        {
            // Act
            Action action = () => _ = new MeteoSwissWeatherService(this.logger, (HttpClient)null, new MeteoSwissApiOptions());

            // Assert
            action.Should().Throw<ArgumentNullException>().WithParameterName("httpClient");
        }

        private MeteoSwissWeatherService CreateMeteoSwissWeatherService(HttpStatusCode statusCode, string responseContent)
        {
            return this.CreateMeteoSwissWeatherService(HttpMessageHandlerMocks.Returning(statusCode, responseContent).Object);
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetWarningIconAsync_DefaultMapping_DownloadsIcon()
        {
            // Arrange
            var httpMessageHandlerMock = HttpMessageHandlerMocks.Returning(HttpStatusCode.OK, "<svg>3</svg>");
            var meteoSwissWeatherService = this.CreateMeteoSwissWeatherService(httpMessageHandlerMock.Object);

            // Act
            using var iconStream = await meteoSwissWeatherService.GetWarningIconAsync(WarnLevel.Level3);

            // Assert
            using var streamReader = new StreamReader(iconStream);
            (await streamReader.ReadToEndAsync()).Should().Be("<svg>3</svg>");
            httpMessageHandlerMock.VerifySendAsync(Times.Once(), r => r.RequestUri.AbsolutePath.EndsWith("/warn-symbols/3.svg"));
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public void Constructor_OptionsIsNull_ThrowsArgumentNullException()
        {
            // Act
            Action action = () => _ = new MeteoSwissWeatherService(this.logger, (IOptions<MeteoSwissApiOptions>)null);

            // Assert
            action.Should().Throw<ArgumentNullException>().WithParameterName("options");
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetCurrentWeatherAsync_BodyNeverArrives_TimesOutAfterHttpClientTimeout()
        {
            // Arrange
            // The headers arrive at once and the body never does: the timeout of the client has to end the download.
            var httpClient = new HttpClient(HttpMessageHandlerMocks.ReturningStallingContent().Object) { Timeout = TimeSpan.FromMilliseconds(200) };
            var meteoSwissWeatherService = new MeteoSwissWeatherService(this.logger, httpClient, new MeteoSwissApiOptions());

            // Act
            Func<Task> action = () => meteoSwissWeatherService.GetCurrentWeatherAsync(6330).WaitAsync(TimeSpan.FromSeconds(10));

            // Assert
            await action.Should().ThrowAsync<OperationCanceledException>();
        }

        private MeteoSwissWeatherService CreateMeteoSwissWeatherService(HttpMessageHandler httpMessageHandler)
        {
            return this.CreateMeteoSwissWeatherService(httpMessageHandler, new MeteoSwissApiOptions());
        }

        private MeteoSwissWeatherService CreateMeteoSwissWeatherService(HttpMessageHandler httpMessageHandler, MeteoSwissApiOptions options)
        {
            var httpClient = new HttpClient(httpMessageHandler);
            return new MeteoSwissWeatherService(this.logger, httpClient, options);
        }
    }
}
