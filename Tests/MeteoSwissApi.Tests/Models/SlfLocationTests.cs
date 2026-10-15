using System.Text.Json;
using MeteoSwissApi.Serialization;

namespace MeteoSwissApi.Tests.Models
{
    [Trait(Traits.Category, Traits.UnitTests)]
    public class SlfLocationTests
    {
        [Fact]
        public void Deserialize_Coordinates_ReturnsLocation()
        {
            // Arrange
            const string json = "{\"type\":\"Point\",\"coordinates\":[9.8194108372,46.6988884749,2558]}";

            // Act
            var slfLocation = JsonSerializer.Deserialize<SlfLocation>(json, JsonSerialization.Default);

            // Assert
            slfLocation.Should().NotBeNull();
            slfLocation.Location.Should().NotBeNull();
            slfLocation.Location!.Latitude.Should().BeApproximately(46.6988884749, 1e-9);
            slfLocation.Location.Longitude.Should().BeApproximately(9.8194108372, 1e-9);
            slfLocation.Location.Altitude.Should().Be(Length.FromMeters(2558));
        }

        [Fact]
        public void Deserialize_CoordinatesOutOfRange_ReturnsNoLocation()
        {
            // Arrange
            const string json = "{\"type\":\"Point\",\"coordinates\":[200,100]}";

            // Act
            var slfLocation = JsonSerializer.Deserialize<SlfLocation>(json, JsonSerialization.Default);

            // Assert
            slfLocation.Should().NotBeNull();
            slfLocation.Location.Should().BeNull();
        }

        [Fact]
        public void Serialize_Location_WritesTypeAndCoordinatesOnly()
        {
            // Arrange
            var slfLocation = JsonSerializer.Deserialize<SlfLocation>("{\"type\":\"Point\",\"coordinates\":[9.5,46.5]}", JsonSerialization.Default);

            // Act
            var json = JsonSerializer.Serialize(slfLocation, JsonSerialization.Default);

            // Assert
            json.Should().Be("{\"type\":\"Point\",\"coordinates\":[9.5,46.5]}");
        }
    }
}
