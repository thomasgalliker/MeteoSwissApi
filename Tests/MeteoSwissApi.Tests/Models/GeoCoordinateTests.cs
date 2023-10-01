using FluentAssertions;
using MeteoSwissApi.Models;
using UnitsNet;
using Xunit;

namespace MeteoSwissApi.Tests.Models
{
    public class GeoCoordinateTests
    {
        [Fact]
        public void ShouldGetDistanceTo()
        {
            // Arrange
            var geoCoordinate1 = new GeoCoordinate(47.1486137d, 8.5539378d);
            var geoCoordinate2 = new GeoCoordinate(47.1823761d, 8.4611036d);

            // Act
            var distance = geoCoordinate1.GetDistanceTo(geoCoordinate2);

            // Assert
            distance.Should().Be(Length.FromKilometers(7.9661124087069961d));
        }
    }
}
