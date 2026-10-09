namespace MeteoSwissApi
{
    /// <summary>
    /// Reads the stations and measurements of the WSL Institute for Snow and Avalanche Research SLF.
    /// </summary>
    public interface ISlfDataService
    {
        /// <summary>
        /// Gets infos about the weather station given in parameters <paramref name="network"/> and <paramref name="stationCode"/>.
        /// </summary>
        /// <param name="network">The network identifier to which the weather station belongs to.</param>
        /// <param name="stationCode">The identifier of the weather station.</param>
        /// <param name="cancellationToken">Cancels the request.</param>
        /// <exception cref="ArgumentNullException"><paramref name="network"/> or <paramref name="stationCode"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="network"/> or <paramref name="stationCode"/> is blank or contains '/', '\', '?', '#', '%' or '..'.</exception>
        /// <exception cref="MeteoSwissApiException">The request failed or its response could not be read.</exception>
        /// <exception cref="OperationCanceledException">The request was cancelled or timed out.</exception>
        Task<SlfStationInfo> GetStationInfoAsync(string network, string stationCode, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a list of latest measurements from all operational SLF weather stations.
        /// </summary>
        /// <param name="cancellationToken">Cancels the requests.</param>
        /// <exception cref="MeteoSwissApiException">A request failed or its response could not be read.</exception>
        /// <exception cref="OperationCanceledException">The requests were cancelled or timed out.</exception>
        Task<IReadOnlyList<SlfStationMeasurement>> GetLatestMeasurementsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the latest measurements from the SLF weather station with <paramref name="network"/> / <paramref name="stationCode"/>.
        /// </summary>
        /// <param name="network">The code of the measurement network.</param>
        /// <param name="stationCode">The station code.</param>
        /// <param name="cancellationToken">Cancels the requests.</param>
        /// <returns>The latest measurement of the selected weather station.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="network"/> or <paramref name="stationCode"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="network"/> or <paramref name="stationCode"/> is blank or contains '/', '\', '?', '#', '%' or '..'.</exception>
        /// <exception cref="MeteoSwissApiException">A request failed or its response could not be read.</exception>
        /// <exception cref="OperationCanceledException">The requests were cancelled or timed out.</exception>
        Task<SlfStationMeasurement> GetLatestMeasurementByStationCodeAsync(string network, string stationCode, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the measurements from past seven days of the SLF weather station with <paramref name="network"/> / <paramref name="stationCode"/>.
        /// </summary>
        /// <param name="network">The code of the measurement network.</param>
        /// <param name="stationCode">The station code.</param>
        /// <param name="cancellationToken">Cancels the request.</param>
        /// <returns>The measurements of the selected weather station, in ascending order of their date.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="network"/> or <paramref name="stationCode"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="network"/> or <paramref name="stationCode"/> is blank or contains '/', '\', '?', '#', '%' or '..'.</exception>
        /// <exception cref="MeteoSwissApiException">The request failed or its response could not be read.</exception>
        /// <exception cref="OperationCanceledException">The request was cancelled or timed out.</exception>
        Task<IReadOnlyList<SlfStationMeasurementItem>> GetMeasurementsByStationCodeAsync(string network, string stationCode, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns a WebP image stream with a teaser image of the map
        /// where <paramref name="network"/> / <paramref name="stationCode"/> is located.
        /// </summary>
        /// <param name="network">The network identifier to which the weather station belongs to.</param>
        /// <param name="stationCode">The identifier of the weather station.</param>
        /// <param name="cancellationToken">Cancels the request.</param>
        /// <returns>The teaser image as WebP stream. The caller disposes the stream.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="network"/> or <paramref name="stationCode"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="network"/> or <paramref name="stationCode"/> is blank or contains '/', '\', '?', '#', '%' or '..'.</exception>
        /// <exception cref="MeteoSwissApiException">The request failed or its response could not be read.</exception>
        /// <exception cref="OperationCanceledException">The request was cancelled or timed out.</exception>
        Task<Stream> GetMapTeaserImageAsync(string network, string stationCode, CancellationToken cancellationToken = default);
    }
}
