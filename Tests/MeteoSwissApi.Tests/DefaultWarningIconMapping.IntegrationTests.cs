namespace MeteoSwissApi.Tests
{
    /// <summary>
    /// The tests in this file download the live MeteoSwiss icons.
    /// </summary>
    [Collection(Collections.IntegrationTests)]
    public partial class DefaultWarningIconMappingTests
    {
        [Fact]
        [Trait(Traits.Category, Traits.IntegrationTests)]
        public async Task GetIconAsync_Live_DownloadsEveryExistingIcon()
        {
            // Arrange
            var range = Enumerable.Range(2, 4).ToArray();

            var httpClient = new HttpClient();
            var warningIconMapping = new DefaultWarningIconMapping(httpClient);

            // Act
            var downloadedIcons = await TestHelper.TryGetIconsAsync(range, warnLevel => warningIconMapping.GetIconAsync(WarnLevel.FromValue(warnLevel)));

            // Assert
            downloadedIcons.Should().HaveCount(range.Length);

            foreach (var (iconId, stream) in downloadedIcons)
            {
                this.testHelper.WriteFile(
                    stream,
                    fileName: $"meteoswiss_warning_icon_{iconId}",
                    fileExtension: IconFileExtension);
            }
        }
    }
}
