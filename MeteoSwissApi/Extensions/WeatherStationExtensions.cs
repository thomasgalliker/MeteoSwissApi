namespace MeteoSwissApi.Extensions
{
    public static class WeatherStationExtensions
    {
        /// <summary>
        /// Gets the weather stations within <paramref name="maxRadius"/> of <paramref name="location"/>, nearest first.
        /// Stations without a location are left out.
        /// </summary>
        /// <returns>
        /// The <see cref="WeatherStation"/>s with their distance to <paramref name="location"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="weatherStations"/> or <paramref name="location"/> is null.</exception>
        public static IEnumerable<(WeatherStation WeatherStation, Length Distance)> Nearby(this IEnumerable<WeatherStation> weatherStations, GeoCoordinate location, Length maxRadius)
        {
            if (weatherStations is null)
            {
                throw new ArgumentNullException(nameof(weatherStations));
            }

            if (location is null)
            {
                throw new ArgumentNullException(nameof(location));
            }

            var weatherStationsWithDistance = weatherStations
                .Select(s => new { WeatherStation = s, Location = s.Location })
                .Where(x => x.Location is not null)
                .Select(x => (x.WeatherStation, Distance: x.Location!.GetDistanceTo(location)))
                .Where(d => d.Distance <= maxRadius)
                .OrderBy(d => d.Distance)
                .ToList();

            return weatherStationsWithDistance;
        }

        /// <summary>
        /// Gets the weather station nearest to <paramref name="location"/>.
        /// Stations without a location are left out.
        /// </summary>
        /// <returns>
        /// The nearest <see cref="WeatherStation"/> with its distance to <paramref name="location"/>,
        /// or <c>null</c> if there is no station with a location.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="weatherStations"/> or <paramref name="location"/> is null.</exception>
        public static (WeatherStation WeatherStation, Length Distance)? Nearest(this IEnumerable<WeatherStation> weatherStations, GeoCoordinate location)
        {
            if (weatherStations is null)
            {
                throw new ArgumentNullException(nameof(weatherStations));
            }

            if (location is null)
            {
                throw new ArgumentNullException(nameof(location));
            }

            (WeatherStation WeatherStation, Length Distance)? nearest = null;

            foreach (var weatherStation in weatherStations)
            {
                if (weatherStation.Location is not GeoCoordinate stationLocation)
                {
                    continue;
                }

                var distance = stationLocation.GetDistanceTo(location);
                if (nearest is null || distance < nearest.Value.Distance)
                {
                    nearest = (weatherStation, distance);
                }
            }

            return nearest;
        }
    }
}