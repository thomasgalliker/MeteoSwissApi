namespace MeteoSwissApi.Tests
{
    public partial class DefaultWarningIconMappingTests
    {
        private const string IconFileExtension = "svg";

        private readonly TestHelper testHelper;

        public DefaultWarningIconMappingTests(ITestOutputHelper testOutputHelper)
        {
            this.testHelper = new TestHelper(testOutputHelper);
        }

        [Theory]
        [Trait(Traits.Category, Traits.UnitTests)]
        [InlineData(0)] // WarnLevel.NoWarnLevel
        [InlineData(1)] // WarnLevel.Level1 has no warning icon
        public async Task GetIconAsync_WarnLevelWithoutIcon_ReturnsTransparentIconWithoutRequest(int warnLevel)
        {
            // Arrange
            var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            var warningIconMapping = new DefaultWarningIconMapping(new HttpClient(httpMessageHandlerMock.Object));

            // Act
            using var stream = await warningIconMapping.GetIconAsync(WarnLevel.FromValue(warnLevel));

            // Assert
            TestHelper.IsTransparentIcon(stream).Should().BeTrue();
            httpMessageHandlerMock.VerifySendAsync(Times.Never());
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetIconAsync_IconExists_ReturnsIcon()
        {
            // Arrange
            var httpMessageHandlerMock = HttpMessageHandlerMocks.Returning(HttpStatusCode.OK, "<svg>3</svg>");
            var warningIconMapping = new DefaultWarningIconMapping(new HttpClient(httpMessageHandlerMock.Object));

            // Act
            using var stream = await warningIconMapping.GetIconAsync(WarnLevel.Level3);

            // Assert
            using var streamReader = new StreamReader(stream);
            (await streamReader.ReadToEndAsync()).Should().Be("<svg>3</svg>");
            httpMessageHandlerMock.VerifySendAsync(Times.Once(), r => r.RequestUri.AbsolutePath.EndsWith("/warn-symbols/3.svg"));
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetIconAsync_IconDoesNotExist_ReturnsTransparentIcon()
        {
            // Arrange
            var warningIconMapping = new DefaultWarningIconMapping(new HttpClient(HttpMessageHandlerMocks.Returning(HttpStatusCode.NotFound, string.Empty).Object));

            // Act
            using var stream = await warningIconMapping.GetIconAsync(WarnLevel.Level3);

            // Assert
            TestHelper.IsTransparentIcon(stream).Should().BeTrue();
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetIconAsync_ServerError_ThrowsMeteoSwissApiException()
        {
            // Arrange
            var warningIconMapping = new DefaultWarningIconMapping(new HttpClient(HttpMessageHandlerMocks.Returning(HttpStatusCode.InternalServerError, string.Empty).Object));

            // Act
            Func<Task> action = () => warningIconMapping.GetIconAsync(WarnLevel.Level3);

            // Assert
            var exception = (await action.Should().ThrowAsync<MeteoSwissApiException>()).Which;
            exception.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public void Constructor_HttpClientIsNull_ThrowsArgumentNullException()
        {
            // Act
            Action action = () => _ = new DefaultWarningIconMapping(null);

            // Assert
            action.Should().Throw<ArgumentNullException>();
        }
    }
}