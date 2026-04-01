using System.Collections.Generic;
using System.Linq;
using MeteoSwissApi.Models;
using UnitsNet;

namespace MeteoSwissApi.Extensions
{
    public static class WeatherStationExtensions
    {
        /// <summary>
        /// Gets weather stations which are nearby <paramref name="location"/> 
        /// with a maxiumum radius of <paramref name="maxRadius"/>.
        /// </summary>
        /// <returns>
        /// List of <see cref="WeatherStation"/> with the respective distance to the given <paramref name="location"/>.
        /// </returns>
        public static IEnumerable<(WeatherStation WeatherStation, Length Distance)> Nearby(this IEnumerable<WeatherStation> weatherStations, GeoCoordinate location, Length maxRadius)
        {
            var weatherStationsWithDistance = weatherStations
                .Select(s => new { WeatherStation = s, Location = s.Location })
                .Where(x => x.Location is not null)
                .Select(x => (x.WeatherStation, Distance: x.Location!.GetDistanceTo(location)))
                .Where(d => d.Distance <= maxRadius)
                .OrderBy(d => d.Distance)
                .ToList();

            return weatherStationsWithDistance;
        }
    }
}