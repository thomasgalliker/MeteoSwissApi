using System.Linq;
using System.Net.Http;
using MeteoSwissApi.Tests.Utils;
using Xunit;
using Xunit.Abstractions;

namespace MeteoSwissApi.Tests
{
    public class DefaultWeatherIconMappingTests
    {
        private const string IconFileExtension = "svg";

        private readonly TestHelper testHelper;

        public DefaultWeatherIconMappingTests(ITestOutputHelper testOutputHelper)
        {
            this.testHelper = new TestHelper(testOutputHelper);
        }

        [Fact]
        public async void ShouldDownloadAllExistingIcons()
        {
            // Arrange
            var range = Enumerable.Range(1, 200).ToArray();

            var httpClient = new HttpClient();
            var weatherIconMapping = new DefaultWeatherIconMapping(httpClient);

            // Act
            var downloadedIcons = await TestHelper.TryGetIconsAsync(range, weatherIconMapping.GetIconAsync);

            // Assert
            foreach (var (IconId, Stream) in downloadedIcons)
            {
                this.testHelper.WriteFile(
                    Stream,
                    fileName: $"meteoswiss_icon_{IconId:000}",
                    fileExtension: IconFileExtension);
            }
        }
    }
}
