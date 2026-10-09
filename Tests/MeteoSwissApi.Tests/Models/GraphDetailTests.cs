using System.Text.Json;
using MeteoSwissApi.Serialization;

namespace MeteoSwissApi.Tests.Models
{
    [Trait(Traits.Category, Traits.UnitTests)]
    public class GraphDetailTests
    {
        [Fact]
        public void Deserialize_ArraysAreNull_ReturnsEmptyArrays()
        {
            // Arrange
            const string json = "{\"temperatureMin1h\":null,\"windSpeed1h\":null,\"sunrise\":null,\"weatherIcon3hV2\":null}";

            // Act
            var graphDetail = JsonSerializer.Deserialize<GraphDetail>(json, JsonSerialization.Default);

            // Assert
            graphDetail.TemperatureMin1h.Should().BeEmpty();
            graphDetail.WindSpeed1h.Should().BeEmpty();
            graphDetail.Sunrise.Should().BeEmpty();
            graphDetail.WeatherIcon3hV2.Should().BeEmpty();
        }
    }
}
