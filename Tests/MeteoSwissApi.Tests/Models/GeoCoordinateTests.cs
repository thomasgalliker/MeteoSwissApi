namespace MeteoSwissApi.Tests.Models
{
    [Trait(Traits.Category, Traits.UnitTests)]
    public class GeoCoordinateTests
    {
        [Fact]
        public void GetDistanceTo_TwoCoordinates_ReturnsGreatCircleDistance()
        {
            // Arrange
            var geoCoordinate1 = new GeoCoordinate(47.1486137d, 8.5539378d);
            var geoCoordinate2 = new GeoCoordinate(47.1823761d, 8.4611036d);

            // Act
            var distance = geoCoordinate1.GetDistanceTo(geoCoordinate2);

            // Assert
            distance.Kilometers.Should().BeApproximately(7.9661124087069961d, 1e-9);
        }

        [Fact]
        public void GetDistanceTo_OtherIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            var geoCoordinate = new GeoCoordinate(47.1486137d, 8.5539378d);

            // Act
            Action action = () => geoCoordinate.GetDistanceTo(null);

            // Assert
            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetDistanceTo_OtherIsUnknown_ThrowsArgumentException()
        {
            // Arrange
            var geoCoordinate = new GeoCoordinate(47.1486137d, 8.5539378d);

            // Act
            Action action = () => geoCoordinate.GetDistanceTo(GeoCoordinate.Unknown);

            // Assert
            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Unknown_ChangedByCaller_StaysUnknownForOthers()
        {
            // Arrange
            var changedUnknown = GeoCoordinate.Unknown;
            changedUnknown.Latitude = 47;
            changedUnknown.Longitude = 8;

            // Act
            var unknown = GeoCoordinate.Unknown;

            // Assert
            unknown.IsUnknown.Should().BeTrue();
            changedUnknown.IsUnknown.Should().BeFalse();
        }

        [Fact]
        public void ToString_Unknown_ReturnsUnknown()
        {
            // Act
            var text = GeoCoordinate.Unknown.ToString();

            // Assert
            text.Should().Be("Unknown");
        }

        [Fact]
        public void ToString_Coordinate_ReturnsInvariantLatitudeAndLongitude()
        {
            // Arrange
            using var cultureScope = new CultureScope("de-DE");
            var geoCoordinate = new GeoCoordinate(47.5d, 8.25d);

            // Act
            var text = geoCoordinate.ToString();

            // Assert
            text.Should().Be("47.5, 8.25");
        }
    }
}
