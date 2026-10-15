namespace MeteoSwissApi.Tests
{
    /// <summary>
    /// The tests in this file download the live MeteoSwiss icons.
    /// </summary>
    [Collection(Collections.IntegrationTests)]
    public partial class DefaultWeatherIconMappingTests
    {
        [Fact]
        [Trait(Traits.Category, Traits.IntegrationTests)]
        public async Task GetIconAsync_Live_DownloadsEveryExistingIcon()
        {
            // Arrange
            var range = Enumerable.Range(1, 200).ToArray();

            var httpClient = new HttpClient();
            var weatherIconMapping = new DefaultWeatherIconMapping(httpClient);

            // Act
            var downloadedIcons = await TestHelper.TryGetIconsAsync(range, iconId => weatherIconMapping.GetIconAsync(iconId));

            // Assert
            downloadedIcons.Should().NotBeEmpty();

            foreach (var (iconId, stream) in downloadedIcons)
            {
                this.testHelper.WriteFile(
                    stream,
                    fileName: $"meteoswiss_icon_{iconId:000}",
                    fileExtension: IconFileExtension);
            }
        }
    }
}
