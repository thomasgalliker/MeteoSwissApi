namespace MeteoSwissApi
{
    /// <summary>
    /// Reads the stations and the latest measurements of the automatic measuring network SwissMetNet of MeteoSwiss.
    /// </summary>
    public interface ISwissMetNetService
    {
        /// <summary>
        /// Gets all weather stations of the automatic measuring network SwissMetNet.
        /// The stations are cached for <see cref="SwissMetNetOptions.CacheExpiration"/> after the download, if it is set.
        /// </summary>
        /// <param name="cancellationToken">Cancels the request.</param>
        /// <exception cref="MeteoSwissApiException">The request failed or its response could not be read.</exception>
        /// <exception cref="OperationCanceledException">The request was cancelled or timed out.</exception>
        Task<IReadOnlyList<WeatherStation>> GetWeatherStationsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all weather stations of the automatic measuring network SwissMetNet.
        /// </summary>
        /// <param name="cacheExpiration">
        /// How long the stations are cached after the download, or <c>null</c> for <see cref="SwissMetNetOptions.CacheExpiration"/>.
        /// The stations change rarely, so a long duration such as <c>TimeSpan.FromDays(30)</c> is fine.
        /// </param>
        /// <param name="cancellationToken">Cancels the request.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="cacheExpiration"/> is zero or negative.</exception>
        /// <exception cref="MeteoSwissApiException">The request failed or its response could not be read.</exception>
        /// <exception cref="OperationCanceledException">The request was cancelled or timed out.</exception>
        Task<IReadOnlyList<WeatherStation>> GetWeatherStationsAsync(TimeSpan? cacheExpiration, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the weather station with <paramref name="stationCode"/>, or <c>null</c> if there is no such station.
        /// The stations are cached for <see cref="SwissMetNetOptions.CacheExpiration"/> after the download, if it is set.
        /// </summary>
        /// <param name="stationCode">The station code of the weather station, compared ignoring case.</param>
        /// <param name="cancellationToken">Cancels the request.</param>
        /// <exception cref="ArgumentNullException"><paramref name="stationCode"/> is null.</exception>
        /// <exception cref="MeteoSwissApiException">The request failed or its response could not be read.</exception>
        /// <exception cref="OperationCanceledException">The request was cancelled or timed out.</exception>
        Task<WeatherStation?> GetWeatherStationAsync(string stationCode, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the weather station with <paramref name="stationCode"/>, or <c>null</c> if there is no such station.
        /// </summary>
        /// <param name="stationCode">The station code of the weather station, compared ignoring case.</param>
        /// <param name="cacheExpiration">
        /// How long the stations are cached after the download, or <c>null</c> for <see cref="SwissMetNetOptions.CacheExpiration"/>.
        /// The stations change rarely, so a long duration such as <c>TimeSpan.FromDays(30)</c> is fine.
        /// </param>
        /// <param name="cancellationToken">Cancels the request.</param>
        /// <exception cref="ArgumentNullException"><paramref name="stationCode"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="cacheExpiration"/> is zero or negative.</exception>
        /// <exception cref="MeteoSwissApiException">The request failed or its response could not be read.</exception>
        /// <exception cref="OperationCanceledException">The request was cancelled or timed out.</exception>
        Task<WeatherStation?> GetWeatherStationAsync(string stationCode, TimeSpan? cacheExpiration, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the latest measurement of every SwissMetNet weather station. MeteoSwiss publishes new values every 10 minutes.
        /// The measurements are cached until the newest of them is <see cref="SwissMetNetOptions.CacheExpiration"/> old, if it is set.
        /// </summary>
        /// <param name="cancellationToken">Cancels the request.</param>
        /// <returns>The latest measurement of every station.</returns>
        /// <exception cref="MeteoSwissApiException">The request failed or its response could not be read.</exception>
        /// <exception cref="OperationCanceledException">The request was cancelled or timed out.</exception>
        Task<IReadOnlyList<WeatherStationMeasurement>> GetLatestMeasurementsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the latest measurement of every SwissMetNet weather station. MeteoSwiss publishes new values every 10 minutes.
        /// </summary>
        /// <param name="cacheExpiration">
        /// How long the measurements are cached, counted from the newest measurement, or <c>null</c> for <see cref="SwissMetNetOptions.CacheExpiration"/>.
        /// With <c>TimeSpan.FromMinutes(20)</c>, measurements dated 10:00 are cached until 10:20; after that, every call downloads
        /// until MeteoSwiss publishes newer values.
        /// </param>
        /// <param name="cancellationToken">Cancels the request.</param>
        /// <returns>The latest measurement of every station.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="cacheExpiration"/> is zero or negative.</exception>
        /// <exception cref="MeteoSwissApiException">The request failed or its response could not be read.</exception>
        /// <exception cref="OperationCanceledException">The request was cancelled or timed out.</exception>
        Task<IReadOnlyList<WeatherStationMeasurement>> GetLatestMeasurementsAsync(TimeSpan? cacheExpiration, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the latest measurement of the weather station with <paramref name="stationCode"/>, or <c>null</c> if there is no such station.
        /// The measurements are cached until the newest of them is <see cref="SwissMetNetOptions.CacheExpiration"/> old, if it is set.
        /// </summary>
        /// <param name="stationCode">The station code, compared ignoring case.</param>
        /// <param name="cancellationToken">Cancels the request.</param>
        /// <returns>The latest measurement of the station.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="stationCode"/> is null.</exception>
        /// <exception cref="MeteoSwissApiException">The request failed or its response could not be read.</exception>
        /// <exception cref="OperationCanceledException">The request was cancelled or timed out.</exception>
        Task<WeatherStationMeasurement?> GetLatestMeasurementAsync(string stationCode, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the latest measurement of the weather station with <paramref name="stationCode"/>, or <c>null</c> if there is no such station.
        /// </summary>
        /// <param name="stationCode">The station code, compared ignoring case.</param>
        /// <param name="cacheExpiration">
        /// How long the measurements are cached, counted from the newest measurement, or <c>null</c> for <see cref="SwissMetNetOptions.CacheExpiration"/>.
        /// With <c>TimeSpan.FromMinutes(20)</c>, measurements dated 10:00 are cached until 10:20; after that, every call downloads
        /// until MeteoSwiss publishes newer values.
        /// </param>
        /// <param name="cancellationToken">Cancels the request.</param>
        /// <returns>The latest measurement of the station.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="stationCode"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="cacheExpiration"/> is zero or negative.</exception>
        /// <exception cref="MeteoSwissApiException">The request failed or its response could not be read.</exception>
        /// <exception cref="OperationCanceledException">The request was cancelled or timed out.</exception>
        Task<WeatherStationMeasurement?> GetLatestMeasurementAsync(string stationCode, TimeSpan? cacheExpiration, CancellationToken cancellationToken = default);
    }
}