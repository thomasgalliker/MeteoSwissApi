using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MeteoSwissApi.Models;

namespace MeteoSwissApi
{
    public interface ISlfDataService
    {
        /// <summary>
        /// Gets infos about the weather station given in parameters <paramref name="network"/> and <paramref name="stationCode"/>.
        /// </summary>
        /// <param name="network">The network identifier to which the weather station belongs to.</param>
        /// <param name="stationCode">The identifier of the weather station.</param>
        Task<SlfStationInfo> GetStationInfoAsync(string network, string stationCode);

        /// <summary>
        /// Gets a list of latest measurements from all operational SLF weather stations.
        /// </summary>
        Task<IEnumerable<SlfStationMeasurement>> GetLatestMeasurementsAsync();

        /// <summary>
        /// Gets the latest measurements from the SLF weather station with <paramref name="network"/> / <paramref name="stationCode"/>.
        /// </summary>
        /// <param name="network">The code of the measurement network.</param>
        /// <param name="stationCode">The station code.</param>
        /// <returns>The latest measurement of the selected weather station.</returns>
        Task<SlfStationMeasurement> GetLatestMeasurementByStationCodeAsync(string network, string stationCode);

        /// <summary>
        /// Gets the measurements from past seven days of the SLF weather station with <paramref name="network"/> / <paramref name="stationCode"/>.
        /// </summary>
        /// <param name="network">The code of the measurement network.</param>
        /// <param name="stationCode">The station code.</param>
        /// <returns>The measurements of the selected weather station.</returns>
        Task<IEnumerable<SlfStationMeasurementItem>> GetMeasurementsByStationCodeAsync(string network, string code);

        /// <summary>
        /// Returns a bitmap stream with a teaser image of the map
        /// where <paramref name="network"/> / <paramref name="stationCode"/> is located.
        /// </summary>
        /// <param name="network">The network identifier to which the weather station belongs to.</param>
        /// <param name="stationCode">The identifier of the weather station.</param>
        Task<Stream> GetMapTeaserImageAsync(string network, string stationCode);
    }
}
