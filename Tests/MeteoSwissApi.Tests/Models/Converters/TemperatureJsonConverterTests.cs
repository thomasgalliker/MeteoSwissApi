using System.Text.Json;
using MeteoSwissApi.Models.Converters;
using MeteoSwissApi.Serialization;

namespace MeteoSwissApi.Tests.Models.Converters
{
    [Trait(Traits.Category, Traits.UnitTests)]
    public class TemperatureJsonConverterTests
    {
        private static JsonSerializerOptions CreateOptions()
        {
            return new JsonSerializerOptions { Converters = { new TemperatureJsonConverter() } };
        }

        [Theory]
        [InlineData("14.6", 14.6)]
        [InlineData("-0.1", -0.1)]
        [InlineData("\"14.6\"", 14.6)]  // numeric string
        [InlineData("\"-0.1\"", -0.1)]
        [InlineData("32767", 32767)]    // the 'no data' sentinel is NOT special-cased here (unlike the nullable converter)
        public void ShouldReadTemperature(string jsonValue, double expectedCelsius)
        {
            // Arrange
            var options = CreateOptions();

            // Act
            var temperature = JsonSerializer.Deserialize<Temperature>(jsonValue, options);

            // Assert
            temperature.Should().Be(Temperature.FromDegreesCelsius(expectedCelsius));
        }

        [Fact]
        public void ShouldReadTemperature_FallsBackToDefault_ForUnsupportedToken()
        {
            // Arrange
            var options = CreateOptions();

            // Act
            var temperature = JsonSerializer.Deserialize<Temperature>("true", options);

            // Assert
            temperature.Should().Be(default(Temperature));
        }

        [Fact]
        public void ShouldWriteTemperature_AsInvariantString()
        {
            // Arrange
            var options = CreateOptions();

            // Act
            var json = JsonSerializer.Serialize(Temperature.FromDegreesCelsius(14.6), options);

            // Assert
            json.Should().Be("\"14.6\"");
        }

        [Fact]
        public void ShouldDeserializeForecast_ParsesMinAndMaxTemperature()
        {
            // Arrange
            const string json = "{\"temperatureMax\":24.6,\"temperatureMin\":\"12.3\"}";

            // Act
            var forecast = JsonSerializer.Deserialize<Forecast>(json, JsonSerialization.CreateOptions());

            // Assert
            forecast.Should().NotBeNull();
            forecast.TemperatureMax.Should().Be(Temperature.FromDegreesCelsius(24.6));
            forecast.TemperatureMin.Should().Be(Temperature.FromDegreesCelsius(12.3));
        }
    }
}
