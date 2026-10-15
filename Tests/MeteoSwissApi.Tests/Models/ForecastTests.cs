using System.Text.Json;
using MeteoSwissApi.Serialization;

namespace MeteoSwissApi.Tests.Models
{
    [Trait(Traits.Category, Traits.UnitTests)]
    public class ForecastTests
    {
        [Theory]
        [InlineData("null")]
        [InlineData("32767")]
        [InlineData("\"\"")]
        public void Deserialize_NoDataValue_ReturnsNullValues(string noDataValue)
        {
            // Arrange
            var json = "{\"dayDate\":\"2026-07-19\"," +
                       $"\"temperatureMax\":{noDataValue},\"temperatureMin\":{noDataValue}," +
                       $"\"precipitation\":{noDataValue},\"precipitationMin\":{noDataValue},\"precipitationMax\":{noDataValue}}}";

            // Act
            var forecast = JsonSerializer.Deserialize<Forecast>(json, JsonSerialization.Default);

            // Assert
            forecast.Should().NotBeNull();
            forecast.TemperatureMax.Should().BeNull();
            forecast.TemperatureMin.Should().BeNull();
            forecast.Precipitation.Should().BeNull();
            forecast.PrecipitationMin.Should().BeNull();
            forecast.PrecipitationMax.Should().BeNull();
        }

        [Fact]
        public void Deserialize_ValuesArePresent_ReturnsValues()
        {
            // Arrange
            const string json = "{\"dayDate\":\"2026-07-19\",\"temperatureMax\":26,\"temperatureMin\":18," +
                                "\"precipitation\":0.2,\"precipitationMin\":0.1,\"precipitationMax\":1.5}";

            // Act
            var forecast = JsonSerializer.Deserialize<Forecast>(json, JsonSerialization.Default);

            // Assert
            forecast.Should().NotBeNull();
            forecast.TemperatureMax.Should().Be(Temperature.FromDegreesCelsius(26));
            forecast.TemperatureMin.Should().Be(Temperature.FromDegreesCelsius(18));
            forecast.Precipitation.Should().Be(Length.FromMillimeters(0.2));
            forecast.PrecipitationMin.Should().Be(Length.FromMillimeters(0.1));
            forecast.PrecipitationMax.Should().Be(Length.FromMillimeters(1.5));
        }
        [Theory]
        [InlineData("null")]
        [InlineData("32767")]
        public void Deserialize_NoDayIcon_ReturnsNullIconAndUnknownCondition(string noIcon)
        {
            // Arrange
            var json = $"{{\"dayDate\":\"2026-07-19\",\"iconDay\":{noIcon},\"iconDayV2\":{noIcon}}}";

            // Act
            var forecast = JsonSerializer.Deserialize<Forecast>(json, JsonSerialization.Default)!;

            // Assert
            forecast.IconDayV2.Should().BeNull();
            forecast.WeatherCondition.Should().Be(WeatherConditionCode.Unknown);
        }

        [Fact]
        public void Deserialize_DayIcon_ReturnsIconAndCondition()
        {
            // Arrange
            const string json = "{\"dayDate\":\"2026-07-19\",\"iconDayV2\":1}";

            // Act
            var forecast = JsonSerializer.Deserialize<Forecast>(json, JsonSerialization.Default)!;

            // Assert
            forecast.IconDayV2.Should().Be(1);
            forecast.WeatherCondition.Should().Be(WeatherConditionCode.Code1);
        }
    }
}
