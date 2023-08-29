using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
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
            var iconDownloadTasks = range.Select(iconId =>
            {
                return Task.Run(async () =>
                {
                    try
                    {
                        return (IconId: iconId, Stream: await weatherIconMapping.GetIconAsync(iconId));
                    }
                    catch (Exception)
                    {
                        return (iconId, null);
                    }
                });
            }).ToArray();

            await Task.WhenAll(iconDownloadTasks);

            var downloadedIcons = (await Task.WhenAll(iconDownloadTasks))
                .Where(x => x.Stream != null)
                .ToArray();


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
