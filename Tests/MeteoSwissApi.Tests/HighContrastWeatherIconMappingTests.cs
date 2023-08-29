using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using MeteoSwissApi.Tests.Utils;
using Xunit;
using Xunit.Abstractions;

namespace MeteoSwissApi.Tests
{
    public class HighContrastWeatherIconMappingTests
    {
        private const string IconFileExtension = "svg";

        private readonly TestHelper testHelper;

        public HighContrastWeatherIconMappingTests(ITestOutputHelper testOutputHelper)
        {
            this.testHelper = new TestHelper(testOutputHelper);
        }

        [Fact]
        public async Task ShouldGetIconAsync()
        {
            // Arrange
            var highContrastWeatherIconMapping = new HighContrastWeatherIconMapping();

            // Act
            var stream = await highContrastWeatherIconMapping.GetIconAsync(1);

            // Assert
            this.testHelper.WriteFile(stream, fileExtension: IconFileExtension);
            stream.Should().NotBeNull();
        }
    }
}
