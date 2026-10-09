using System.Text.Json;
using MeteoSwissApi.Serialization;

namespace MeteoSwissApi.Tests.Models
{
    [Trait(Traits.Category, Traits.UnitTests)]
    public class SlfStationDateValueTests
    {
        [Fact]
        public void Serialize_Temperature_WritesTimestampAndValueOnce()
        {
            // Arrange
            var measurement = new SlfStationDateTemperature
            {
                Date = new DateTime(2026, 10, 3, 9, 30, 0, DateTimeKind.Utc),
                Value = Temperature.FromDegreesCelsius(1),
            };

            // Act
            var json = JsonSerializer.Serialize(measurement, JsonSerialization.Default);

            // Assert
            using var document = JsonDocument.Parse(json);
            document.RootElement.EnumerateObject().Select(p => p.Name).Should().BeEquivalentTo("timestamp", "value");
            document.RootElement.GetProperty("value").GetDouble().Should().Be(1);
        }

        [Fact]
        public void Serialize_MeasurementWithSnowHeight_RoundTrips()
        {
            // Arrange
            var measurement = new SlfStationMeasurement
            {
                SnowHeight = new SlfStationDateLength { Date = new DateTime(2026, 10, 3, 9, 30, 0, DateTimeKind.Utc), Value = Length.FromCentimeters(12) },
            };

            // Act
            var json = JsonSerializer.Serialize(measurement, JsonSerialization.Default);
            var roundTripped = JsonSerializer.Deserialize<SlfStationMeasurement>(json, JsonSerialization.Default);

            // Assert
            roundTripped.SnowHeight.Should().NotBeNull();
            roundTripped.SnowHeight!.Value.Should().Be(Length.FromCentimeters(12));
        }
    }
}
