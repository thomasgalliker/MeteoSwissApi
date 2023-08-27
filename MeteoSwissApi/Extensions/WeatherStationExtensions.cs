using System.Collections.Generic;
using System.Linq;
using MeteoSwissApi.Models;
using UnitsNet;

namespace MeteoSwissApi.Extensions
{
    public static class WeatherStationExtensions
    {
        /// <summary>
        /// Gets weather stations which are nearby <paramref name="location"/> with a maxiumum radius of <paramref name="maxRadius"/>.
        /// </summary>
        public static IEnumerable<(WeatherStation WeatherStation, Length Distance)> Nearby(this IEnumerable<WeatherStation> weatherStations, GeoCoordinate location, Length maxRadius)
        {
            var weatherStationsWithDistance = weatherStations
                .Where(s => s.Location != null)
                .Select(s => (WeatherStation: s, Distance: s.Location.GetDistanceTo(location)))
                .Where(d => d.Distance <= maxRadius)
                .OrderBy(d => d.Distance)
                .ToList();

            return weatherStationsWithDistance;
        }
    }
}
