namespace MeteoSwissApi.Tests
{
    /// <summary>
    /// The tests in this file compare the embedded icons with the live MeteoSwiss icons.
    /// </summary>
    [Collection(Collections.IntegrationTests)]
    public partial class HighContrastWeatherIconMappingTests
    {
        [Fact]
        [Trait(Traits.Category, Traits.IntegrationTests)]
        public async Task GetIconAsync_Live_HasAnIconForEveryMeteoSwissIcon()
        {
            // Arrange
            var highContrastWeatherIconMapping = new HighContrastWeatherIconMapping();

            var range = Enumerable.Range(1, 200).ToArray();

            var httpClient = new HttpClient();
            var weatherIconMapping = new DefaultWeatherIconMapping(httpClient);
            var downloadedIcons = await TestHelper.TryGetIconsAsync(range, iconId => weatherIconMapping.GetIconAsync(iconId));

            async Task<(int Id, bool IsMissing)> GetHighContrastIcon((int IconId, Stream Stream) i)
            {
                // The mapping returns the transparent icon for icon ids without high-contrast icon.
                using var stream = await highContrastWeatherIconMapping.GetIconAsync(i.IconId);
                return (i.IconId, TestHelper.IsTransparentIcon(stream));
            }

            // Act
            var icons = await Task.WhenAll(downloadedIcons.Select(GetHighContrastIcon));

            // Assert
            downloadedIcons.Should().NotBeEmpty();

            var missingIcons = icons
                .Where(c => c.IsMissing)
                .Select(c => c.Id)
                .ToArray();

            missingIcons.Length.Should().Be(
                expected: 0,
                because: $"Following high-contrast icons are missing: " +
                $"[{string.Join(", ", missingIcons)}]");
        }
    }
}
