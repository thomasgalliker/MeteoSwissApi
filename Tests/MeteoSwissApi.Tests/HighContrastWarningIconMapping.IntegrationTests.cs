namespace MeteoSwissApi.Tests
{
    /// <summary>
    /// The tests in this file compare the embedded icons with the live MeteoSwiss icons.
    /// </summary>
    [Collection(Collections.IntegrationTests)]
    public partial class HighContrastWarningIconMappingTests
    {
        [Fact]
        [Trait(Traits.Category, Traits.IntegrationTests)]
        public async Task GetIconAsync_Live_HasAnIconForEveryMeteoSwissIcon()
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
                var response = await httpClient.GetAsync(string.Format(CultureInfo.InvariantCulture, imageApiEndpoint, warnLevel));
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
                using var stream = await highContrastWarningIconMapping.GetIconAsync(WarnLevel.FromValue(iconId));
                TestHelper.IsTransparentIcon(stream).Should().BeFalse(because: $"warn level {iconId} needs a high-contrast icon");
            }
        }
    }
}
