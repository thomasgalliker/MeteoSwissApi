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

        Task<IEnumerable<SlfStationMeasurement>> GetLatestMeasurementsAsync();

        Task<SlfStationMeasurement> GetLatestMeasurementByStationCodeAsync(string network, string stationCode);

        /// <summary>
        /// Returns a bitmap stream with a teaser image of the map
        /// where <paramref name="network"/> / <paramref name="stationCode"/> is located.
        /// </summary>
        /// <param name="network">The network identifier to which the weather station belongs to.</param>
        /// <param name="stationCode">The identifier of the weather station.</param>
        Task<Stream> GetMapTeaserImageAsync(string network, string stationCode);
    }
}
