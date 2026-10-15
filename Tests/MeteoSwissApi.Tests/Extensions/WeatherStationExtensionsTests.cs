namespace MeteoSwissApi.Tests.Extensions
{
    [Trait(Traits.Category, Traits.UnitTests)]
    public class WeatherStationExtensionsTests
    {
        private static readonly GeoCoordinate Cham = new GeoCoordinate(47.1826432d, 8.470528d);

        [Fact]
        public void Nearby_StationsWithinRadius_ReturnsNearestFirst()
        {
            // Arrange
            var zug = new WeatherStation { StationCode = "ZUG", Location = new GeoCoordinate(47.17, 8.52) };
            var lucerne = new WeatherStation { StationCode = "LUZ", Location = new GeoCoordinate(47.04, 8.30) };
            var withoutLocation = new WeatherStation { StationCode = "NON" };
            var farAway = new WeatherStation { StationCode = "GVE", Location = new GeoCoordinate(46.25, 6.13) };

            // Act
            var nearby = new[] { farAway, lucerne, withoutLocation, zug }.Nearby(Cham, Length.FromKilometers(30)).ToArray();

            // Assert
            nearby.Select(n => n.WeatherStation.StationCode).Should().Equal("ZUG", "LUZ");
            nearby[0].Distance.Should().BeLessThan(nearby[1].Distance);
        }

        [Fact]
        public void Nearby_LocationIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            var weatherStations = new[] { new WeatherStation { StationCode = "ZUG", Location = new GeoCoordinate(47.17, 8.52) } };

            // Act
            Action action = () => weatherStations.Nearby(null!, Length.FromKilometers(30));

            // Assert
            action.Should().Throw<ArgumentNullException>().WithParameterName("location");
        }

        [Fact]
        public void Nearby_StationsIsNull_ThrowsArgumentNullException()
        {
            // Act
            Action action = () => WeatherStationExtensions.Nearby(null!, Cham, Length.FromKilometers(30));

            // Assert
            action.Should().Throw<ArgumentNullException>().WithParameterName("weatherStations");
        }
    

        [Fact]
        public void Nearest_StationsWithLocation_ReturnsNearestStation()
        {
            // Arrange
            var zug = new WeatherStation { StationCode = "ZUG", Location = new GeoCoordinate(47.17, 8.52) };
            var lucerne = new WeatherStation { StationCode = "LUZ", Location = new GeoCoordinate(47.04, 8.30) };
            var withoutLocation = new WeatherStation { StationCode = "NON" };
            var farAway = new WeatherStation { StationCode = "GVE", Location = new GeoCoordinate(46.25, 6.13) };

            // Act
            var nearest = new[] { farAway, lucerne, withoutLocation, zug }.Nearest(Cham);

            // Assert
            nearest.Should().NotBeNull();
            nearest!.Value.WeatherStation.Should().BeSameAs(zug);
            nearest.Value.Distance.Should().Be(zug.Location!.GetDistanceTo(Cham));
        }

        [Fact]
        public void Nearest_NoStationWithLocation_ReturnsNull()
        {
            // Arrange
            var weatherStations = new[] { new WeatherStation { StationCode = "NON" } };

            // Act
            var nearest = weatherStations.Nearest(Cham);

            // Assert
            nearest.Should().BeNull();
        }

        [Fact]
        public void Nearest_StationsIsEmpty_ReturnsNull()
        {
            // Act
            var nearest = Array.Empty<WeatherStation>().Nearest(Cham);

            // Assert
            nearest.Should().BeNull();
        }

        [Fact]
        public void Nearest_LocationIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            var weatherStations = new[] { new WeatherStation { StationCode = "ZUG", Location = new GeoCoordinate(47.17, 8.52) } };

            // Act
            Action action = () => weatherStations.Nearest(null!);

            // Assert
            action.Should().Throw<ArgumentNullException>().WithParameterName("location");
        }

        [Fact]
        public void Nearest_StationsIsNull_ThrowsArgumentNullException()
        {
            // Act
            Action action = () => WeatherStationExtensions.Nearest(null!, Cham);

            // Assert
            action.Should().Throw<ArgumentNullException>().WithParameterName("weatherStations");
        }
    }
}
