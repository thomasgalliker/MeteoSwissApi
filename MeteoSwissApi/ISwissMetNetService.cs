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

        Task<IEnumerable<WeatherStationMeasurement>> GetLatestMeasurementsAsync();
    }
}