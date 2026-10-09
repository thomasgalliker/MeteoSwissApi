namespace MeteoSwissApi.Tests
{
    public partial class DefaultWeatherIconMappingTests
    {
        private const string IconFileExtension = "svg";

        private readonly TestHelper testHelper;

        public DefaultWeatherIconMappingTests(ITestOutputHelper testOutputHelper)
        {
            this.testHelper = new TestHelper(testOutputHelper);
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetIconAsync_NoDataIconId_ReturnsTransparentIconWithoutRequest()
        {
            // Arrange
            const int iconId = 32767; // 'No data' sentinel value returned by the MeteoSwiss API

            var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            var weatherIconMapping = new DefaultWeatherIconMapping(new HttpClient(httpMessageHandlerMock.Object));

            // Act
            using var stream = await weatherIconMapping.GetIconAsync(iconId);

            // Assert
            TestHelper.IsTransparentIcon(stream).Should().BeTrue();
            httpMessageHandlerMock.VerifySendAsync(Times.Never());
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetIconAsync_IconExists_ReturnsIcon()
        {
            // Arrange
            var httpMessageHandlerMock = HttpMessageHandlerMocks.Returning(HttpStatusCode.OK, "<svg>1</svg>");
            var weatherIconMapping = new DefaultWeatherIconMapping(new HttpClient(httpMessageHandlerMock.Object));

            // Act
            using var stream = await weatherIconMapping.GetIconAsync(1);

            // Assert
            using var streamReader = new StreamReader(stream);
            (await streamReader.ReadToEndAsync()).Should().Be("<svg>1</svg>");
            httpMessageHandlerMock.VerifySendAsync(Times.Once(), r => r.RequestUri.AbsolutePath.EndsWith("/weather-symbols/1.svg"));
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetIconAsync_IconDoesNotExist_ReturnsTransparentIcon()
        {
            // Arrange
            var weatherIconMapping = new DefaultWeatherIconMapping(new HttpClient(HttpMessageHandlerMocks.Returning(HttpStatusCode.NotFound, string.Empty).Object));

            // Act
            using var stream = await weatherIconMapping.GetIconAsync(999);

            // Assert
            TestHelper.IsTransparentIcon(stream).Should().BeTrue();
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetIconAsync_ServerError_ThrowsMeteoSwissApiException()
        {
            // Arrange
            var weatherIconMapping = new DefaultWeatherIconMapping(new HttpClient(HttpMessageHandlerMocks.Returning(HttpStatusCode.InternalServerError, string.Empty).Object));

            // Act
            Func<Task> action = () => weatherIconMapping.GetIconAsync(1);

            // Assert
            var exception = (await action.Should().ThrowAsync<MeteoSwissApiException>()).Which;
            exception.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetIconAsync_Cancelled_ThrowsOperationCanceledException()
        {
            // Arrange
            using var cancellationTokenSource = new CancellationTokenSource();
            var weatherIconMapping = new DefaultWeatherIconMapping(new HttpClient(HttpMessageHandlerMocks.WaitingForCancellation().Object));

            // Act
            var iconTask = weatherIconMapping.GetIconAsync(1, cancellationTokenSource.Token);
            cancellationTokenSource.Cancel();
            Func<Task> action = () => iconTask;

            // Assert
            await action.Should().ThrowAsync<OperationCanceledException>();
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public void Constructor_HttpClientIsNull_ThrowsArgumentNullException()
        {
            // Act
            Action action = () => _ = new DefaultWeatherIconMapping(null);

            // Assert
            action.Should().Throw<ArgumentNullException>();
        }
    }
}
