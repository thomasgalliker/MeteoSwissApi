namespace MeteoSwissApi.Tests
{
    [Trait(Traits.Category, Traits.UnitTests)]
    public class HighContrastWeatherIconMappingTests
    {
        [Fact]
        public async Task ShouldGetIconAsync()
        {
            // Arrange
            var highContrastWeatherIconMapping = new HighContrastWeatherIconMapping();

            var range = Enumerable.Range(1, 200).ToArray();

            var httpClient = new HttpClient();
            var weatherIconMapping = new DefaultWeatherIconMapping(httpClient);
            var downloadedIcons = await TestHelper.TryGetIconsAsync(range, weatherIconMapping.GetIconAsync);

            async Task<(int Id, Exception Exception)> TryGetHighContrastIcon((int IconId, Stream Stream) i)
            {
                try
                {
                    _ = await highContrastWeatherIconMapping.GetIconAsync(i.IconId);
                    return (i.IconId, null);
                }
                catch (Exception ex)
                {
                    return (i.IconId, ex);
                }
            }

            // Act
            var icons = await Task.WhenAll(downloadedIcons.Select(TryGetHighContrastIcon));

            // Assert
            var iconsWithExceptions = icons
                .Where(c => c.Exception != null)
                .Select(c => c.Id)
                .ToArray();

            iconsWithExceptions.Length.Should().Be(
                expected: 0,
                because: $"Following high-contrast icons are missing: " +
                $"[{string.Join(", ", iconsWithExceptions)}]");
        }

        [Fact]
        public async Task ShouldGetIconAsync_ReturnsTransparentIcon_IfNoDataIconIdIsRequested()
        {
            // Arrange
            const int iconId = 32767; // 'No data' sentinel value returned by the MeteoSwiss API

            var highContrastWeatherIconMapping = new HighContrastWeatherIconMapping();

            // Act
            var stream = await highContrastWeatherIconMapping.GetIconAsync(iconId);

            // Assert
            stream.Should().NotBeNull();

            using var streamReader = new StreamReader(stream);
            var content = streamReader.ReadToEnd();
            content.Should().Contain("width=\"1px\"");
            content.Should().Contain("height=\"1px\"");
        }
    }
}
