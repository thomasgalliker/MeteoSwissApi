using System.Text.Json;
using MeteoSwissApi.Models.Converters;
using MeteoSwissApi.Serialization;

namespace MeteoSwissApi.Tests.Models.Converters
{
    [Trait(Traits.Category, Traits.UnitTests)]
    public class NullableIconJsonConverterTests
    {
        [Fact]
        public void Deserialize_CurrentWeatherWithIconSentinel_ReturnsNullIcon()
        {
            // Arrange
            // Real-world frozen 'currentWeather' block: icon carries the 32767 sentinel while iconV2 is still valid.
            const string json = "{\"time\":1784356800000,\"icon\":32767,\"iconV2\":3,\"temperature\":32767.0}";

            // Act
            var currentWeather = JsonSerializer.Deserialize<CurrentWeather>(json, JsonSerialization.Default);

            // Assert
            currentWeather.Should().NotBeNull();
#pragma warning disable CS0618 // The obsolete property stays readable until it is removed.
            currentWeather.Icon.Should().BeNull();
#pragma warning restore CS0618
            currentWeather.IconV2.Should().Be(3);
        }

        [Fact]
        public void Deserialize_CurrentWeatherPlaceholder_ReturnsNullIconAndUnknownCondition()
        {
            // Arrange
            // The placeholder MeteoSwiss sends for an unknown postal code.
            const string json = "{\"time\":null,\"icon\":null,\"iconV2\":null,\"temperature\":null}";

            // Act
            var currentWeather = JsonSerializer.Deserialize<CurrentWeather>(json, JsonSerialization.Default)!;

            // Assert
            currentWeather.IconV2.Should().BeNull();
            currentWeather.WeatherCondition.Should().Be(WeatherConditionCode.Unknown);
            currentWeather.Temperature.Should().BeNull();
        }

        [Theory]
        [InlineData("32767", null)]      // no-data sentinel
        [InlineData("32767.0", null)]    // sentinel sent as a non-integer number
        [InlineData("null", null)]
        [InlineData("3.5", null)]
        [InlineData("\"3\"", null)]
        [InlineData("3", 3)]
        [InlineData("142", 142)]
        public void Read_Value_ReturnsIconIdOrNull(string jsonValue, int? expected)
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
