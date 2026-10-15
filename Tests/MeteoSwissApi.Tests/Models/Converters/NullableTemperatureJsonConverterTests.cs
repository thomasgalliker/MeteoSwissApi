using System.Text.Json;
using MeteoSwissApi.Models.Converters;
using MeteoSwissApi.Serialization;

namespace MeteoSwissApi.Tests.Models.Converters
{
    [Trait(Traits.Category, Traits.UnitTests)]
    public class NullableTemperatureJsonConverterTests
    {
        [Fact]
        public void Deserialize_CurrentWeatherWithNoDataSentinel_ReturnsNullTemperature()
        {
            // Arrange
            // Real-world payload observed when the MeteoSwiss 'currentWeather' block is frozen:
            // temperature (and icon) are returned as the sentinel 32767 (= short.MaxValue / 0x7FFF).
            const string json = "{\"time\":1784356800000,\"icon\":32767,\"iconV2\":3,\"temperature\":32767.0}";

            // Act
            var currentWeather = JsonSerializer.Deserialize<CurrentWeather>(json, JsonSerialization.Default);

            // Assert
            currentWeather.Should().NotBeNull();
            currentWeather.Temperature.Should().BeNull();
            currentWeather.IconV2.Should().Be(3);
        }

        [Fact]
        public void Deserialize_CurrentWeatherWithTemperature_ReturnsTemperature()
        {
            // Arrange
            const string json = "{\"time\":1784356800000,\"icon\":3,\"iconV2\":3,\"temperature\":14.6}";

            // Act
            var currentWeather = JsonSerializer.Deserialize<CurrentWeather>(json, JsonSerialization.Default);

            // Assert
            currentWeather.Should().NotBeNull();
            currentWeather.Temperature.Should().Be(Temperature.FromDegreesCelsius(14.6));
        }

        [Theory]
        [InlineData("14.6", 14.6)]
        [InlineData("\"14.6\"", 14.6)]  // numeric string
        [InlineData("-0.1", -0.1)]
        public void Read_NumberOrNumericString_ReturnsTemperature(string jsonValue, double expectedCelsius)
        {
            // Arrange
            var options = CreateOptions();

            // Act
            var temperature = JsonSerializer.Deserialize<Temperature?>(jsonValue, options);

            // Assert
            temperature.Should().Be(Temperature.FromDegreesCelsius(expectedCelsius));
        }

        [Theory]
        [InlineData("32767")]    // 'no data' sentinel
        [InlineData("32767.0")]  // sentinel sent as a non-integer number
        [InlineData("-32768")]   // magnitude at/above the sentinel
        [InlineData("null")]
        public void Read_MissingValue_ReturnsNull(string jsonValue)
        {
            // Arrange
            var options = CreateOptions();

            // Act
            var temperature = JsonSerializer.Deserialize<Temperature?>(jsonValue, options);

            // Assert
            temperature.Should().BeNull();
        }

        [Fact]
        public void Write_Temperature_WritesCelsiusAsNumber()
        {
            // Arrange
            var options = CreateOptions();

            // Act
            var json = JsonSerializer.Serialize<Temperature?>(Temperature.FromDegreesCelsius(14.6), options);

            // Assert
            json.Should().Be("14.6");
        }

        [Fact]
        public void Write_Null_WritesNull()
        {
            // Arrange
            var options = CreateOptions();

            // Act
            var json = JsonSerializer.Serialize<Temperature?>(null, options);

            // Assert
            json.Should().Be("null");
        }

        private static JsonSerializerOptions CreateOptions()
        {
            return new JsonSerializerOptions { Converters = { new NullableTemperatureJsonConverter() } };
        }
    }
}
