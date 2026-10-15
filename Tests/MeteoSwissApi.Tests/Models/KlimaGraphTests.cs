using System.Text.Json;
using MeteoSwissApi.Serialization;

namespace MeteoSwissApi.Tests.Models
{
    [Trait(Traits.Category, Traits.UnitTests)]
    public class KlimaGraphTests
    {
        [Fact]
        public void Deserialize_TemperatureDeviation_ReturnsTemperatureDifference()
        {
            // Arrange
            const string json = "{\"temperatureGraphValues\":{\"abs\":[14.2],\"norm\":[12.7],\"abweichung\":[1.5]}}";

            // Act
            var klimaGraph = JsonSerializer.Deserialize<KlimaGraph>(json, JsonSerialization.Default);

            // Assert
            klimaGraph.Should().NotBeNull();
            var temperatureGraphValues = klimaGraph.TemperatureGraphValues;
            temperatureGraphValues.Deviation.Should().Equal(TemperatureDelta.FromDegreesCelsius(1.5));
            (temperatureGraphValues.Normal[0] + temperatureGraphValues.Deviation[0]).DegreesCelsius
                .Should().BeApproximately(temperatureGraphValues.Absolute[0].DegreesCelsius, 1e-9);
        }

        [Fact]
        public void Deserialize_DeviationIsNull_ReturnsEmptyArray()
        {
            // Arrange
            const string json = "{\"temperatureGraphValues\":{\"abweichung\":null}}";

            // Act
            var klimaGraph = JsonSerializer.Deserialize<KlimaGraph>(json, JsonSerialization.Default);

            // Assert
            klimaGraph.Should().NotBeNull();
            klimaGraph.TemperatureGraphValues.Deviation.Should().BeEmpty();
        }
    }
}
