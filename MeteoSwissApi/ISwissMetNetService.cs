using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MeteoSwissApi.Models;

namespace MeteoSwissApi
{
    public interface ISwissMetNetService
    {
        /// <summary>
        /// Gets a list of all weather stations
        /// which belong to the automatic measuring network.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<WeatherStation>> GetWeatherStationsAsync();

        /// <summary>
        /// Gets the weather station by <paramref name="stationCode"/>.
        /// </summary>
        /// <param name="stationCode">The station code of the weather station.</param>
        /// <returns></returns>
        Task<WeatherStation> GetWeatherStationAsync(string stationCode);

        /// <summary>
        /// Gets the list of actual measurement values from all weather stations.
        /// </summary>
        /// <param name="cacheExpiration">
        /// Set a sliding cache expiration. A good value is <c>TimeSpan.FromMinutes(20)</c>.</param>
        /// <returns>List of measurement values.</returns>
        Task<IEnumerable<WeatherStationMeasurement>> GetLatestMeasurementsAsync(TimeSpan? cacheExpiration = null);

        /// <summary>
        /// Gets the lastest measurement provided by the weather station with <paramref name="stationCode"/>.
        /// </summary>
        /// <param name="stationCode">The station code.</param>
        /// <param name="cacheExpiration">
        /// Set a sliding cache expiration. A good value is <c>TimeSpan.FromMinutes(20)</c>.</param>
        /// <returns>List of measurement values.</returns>
        Task<WeatherStationMeasurement> GetLatestMeasurementAsync(string stationCode, TimeSpan? cacheExpiration = null);
    }
}