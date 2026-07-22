namespace MeteoSwissApi.Tests
{
    [Trait(Traits.Category, Traits.UnitTests)]
    public class HighContrastWarningIconMappingTests
    {
        [Fact]
        public async Task ShouldGetIconAsync()
        {
            // Arrange
            var highContrastWarningIconMapping = new HighContrastWarningIconMapping();

            // The original source is probed directly (instead of using DefaultWarningIconMapping)
            // so that this test also fails if MeteoSwiss adds warning icons
            // outside the currently known warn levels.
            const string imageApiEndpoint = "https://www.meteoschweiz.admin.ch/static/resources/warn-symbols/{0}.svg";
            var expectedWarnLevelsWithIcons = new[] { 2, 3, 4, 5 };
            var range = Enumerable.Range(0, 11).ToArray();

            var httpClient = new HttpClient();

            async Task<Stream> DownloadIconAsync(int warnLevel)
            {
                var response = await httpClient.GetAsync(string.Format(imageApiEndpoint, warnLevel));
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStreamAsync();
            }

            // Act
            var downloadedIcons = await TestHelper.TryGetIconsAsync(range, DownloadIconAsync);

            // Assert
            downloadedIcons.Select(i => i.IconId).Should().BeEquivalentTo(
                expectedWarnLevelsWithIcons,
                because: "the embedded high-contrast warning icons must be updated whenever MeteoSwiss adds or removes warning icons");

            foreach (var (iconId, _) in downloadedIcons)
            {
                var stream = await highContrastWarningIconMapping.GetIconAsync(iconId);
                stream.Should().NotBeNull();
            }
        }

        [Theory]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        public async Task ShouldGetIconAsync_ReturnsEmbeddedIcon(int warnLevel)
        {
            // Arrange
            var highContrastWarningIconMapping = new HighContrastWarningIconMapping();

            // Act
            var stream = await highContrastWarningIconMapping.GetIconAsync(warnLevel);

            // Assert
            stream.Should().NotBeNull();

            using var streamReader = new StreamReader(stream!);
            var content = await streamReader.ReadToEndAsync();
            content.Should().Contain("<svg");
        }

        [Theory]
        [InlineData(0)] // WarnLevel.NoWarnLevel
        [InlineData(1)] // WarnLevel.Level1 has no warning icon
        public async Task ShouldGetIconAsync_ReturnsTransparentIcon_IfWarnLevelHasNoIcon(int warnLevel)
        {
            // Arrange
            var highContrastWarningIconMapping = new HighContrastWarningIconMapping();

            // Act
            var stream = await highContrastWarningIconMapping.GetIconAsync(warnLevel);

            // Assert
            stream.Should().NotBeNull();

            using var streamReader = new StreamReader(stream);
            var content = await streamReader.ReadToEndAsync();
            content.Should().Contain("width=\"1px\"");
            content.Should().Contain("height=\"1px\"");
        }

        [Fact]
        public async Task ShouldGetIconAsync_Throws_IfWarnLevelIsInvalid()
        {
            // Arrange
            var highContrastWarningIconMapping = new HighContrastWarningIconMapping();

            // Act
            // The implicit int-to-WarnLevel conversion rejects unknown warn levels.
            Func<Task> action = () => highContrastWarningIconMapping.GetIconAsync(99);

            // Assert
            await action.Should().ThrowAsync<ArgumentOutOfRangeException>();
        }
    }
}