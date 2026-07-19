using System.Text.Json;
using MeteoSwissApi.Models.Converters;
using MeteoSwissApi.Serialization;

namespace MeteoSwissApi.Tests.Models.Converters
{
    public class NullableIconJsonConverterTests
    {
        [Fact]
        public void ShouldDeserializeCurrentWeather_MapsIconSentinelToNull()
        {
            // Arrange
            // Real-world frozen 'currentWeather' block: icon carries the 32767 sentinel while iconV2 is still valid.
            const string json = "{\"time\":1784356800000,\"icon\":32767,\"iconV2\":3,\"temperature\":32767.0}";

            // Act
            var currentWeather = JsonSerializer.Deserialize<CurrentWeather>(json, JsonSerialization.CreateOptions());

            // Assert
            currentWeather.Should().NotBeNull();
            currentWeather.Icon.Should().BeNull();
            currentWeather.IconV2.Should().Be(3);
        }


        [Theory]
        [InlineData("32767", null)]      // no-data sentinel
        [InlineData("32767.0", null)]    // sentinel sent as a non-integer number
        [InlineData("null", null)]
        [InlineData("3", 3)]
        [InlineData("142", 142)]
        public void ShouldReadIcon(string jsonValue, int? expected)
        {
            // Arrange
            var options = new JsonSerializerOptions { Converters = { new NullableIconJsonConverter() } };

            // Act
            var icon = JsonSerializer.Deserialize<int?>(jsonValue, options);

            // Assert
            icon.Should().Be(expected);
        }
    }
}
