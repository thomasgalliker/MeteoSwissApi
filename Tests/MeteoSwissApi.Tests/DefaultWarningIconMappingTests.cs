namespace MeteoSwissApi.Tests
{
    [Trait(Traits.Category, Traits.IntegrationTests)]
    public class DefaultWarningIconMappingTests
    {
        private const string IconFileExtension = "svg";

        private readonly TestHelper testHelper;

        public DefaultWarningIconMappingTests(ITestOutputHelper testOutputHelper)
        {
            this.testHelper = new TestHelper(testOutputHelper);
        }

        [Fact]
        public async Task ShouldDownloadAllExistingIcons()
        {
            // Arrange
            var range = Enumerable.Range(2, 4).ToArray();

            var httpClient = new HttpClient();
            var warningIconMapping = new DefaultWarningIconMapping(httpClient);

            // Act
            var downloadedIcons = await TestHelper.TryGetIconsAsync(range, warnLevel => warningIconMapping.GetIconAsync(warnLevel));

            // Assert
            downloadedIcons.Should().HaveCount(range.Length);

            foreach (var (IconId, Stream) in downloadedIcons)
            {
                this.testHelper.WriteFile(
                    Stream,
                    fileName: $"meteoswiss_warning_icon_{IconId}",
                    fileExtension: IconFileExtension);
            }
        }

        [Theory]
        [InlineData(0)] // WarnLevel.NoWarnLevel
        [InlineData(1)] // WarnLevel.Level1 has no warning icon
        public async Task ShouldGetIconAsync_ReturnsTransparentIcon_IfWarnLevelHasNoIcon(int warnLevel)
        {
            // Arrange
            var httpMessageHandlerMock = new Mock<HttpMessageHandler>();

            var httpClient = new HttpClient(httpMessageHandlerMock.Object);
            var warningIconMapping = new DefaultWarningIconMapping(httpClient);

            // Act
            var stream = await warningIconMapping.GetIconAsync(warnLevel);

            // Assert
            stream.Should().NotBeNull();

            using var streamReader = new StreamReader(stream);
            var content = await streamReader.ReadToEndAsync();
            content.Should().Contain("width=\"1px\"");
            content.Should().Contain("height=\"1px\"");

            httpMessageHandlerMock.Protected()
                .Verify("SendAsync", Times.Never(), ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
        }

        [Theory]
        [InlineData(HttpStatusCode.NotFound)]
        [InlineData(HttpStatusCode.InternalServerError)]
        public async Task ShouldGetIconAsync_Throws_IfServerReturnsError(HttpStatusCode httpStatusCode)
        {
            // Arrange
            var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(httpStatusCode));

            var httpClient = new HttpClient(httpMessageHandlerMock.Object);
            var warningIconMapping = new DefaultWarningIconMapping(httpClient);

            // Act
            Func<Task> action = () => warningIconMapping.GetIconAsync(WarnLevel.Level3);

            // Assert
            await action.Should().ThrowAsync<HttpRequestException>();
        }
    }
}