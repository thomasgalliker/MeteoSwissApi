namespace MeteoSwissApi
{
    /// <summary>
    /// Reads the current weather, the forecast and the icons of the MeteoSwiss app API, by Swiss postal code.
    /// </summary>
    public interface IMeteoSwissWeatherService
    {
        /// <summary>
        ///     Gets the current weather for the given postal code <paramref name="plz"/>.
        /// </summary>
        /// <param name="plz">
        ///     The postal code (PLZ) of the location: 4 digits such as 8001,
        ///     or 6 digits including the 2-digit suffix, such as 800100.
        /// </param>
        /// <param name="cancellationToken">Cancels the request.</param>
        /// <returns>The current weather info.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="plz"/> has neither 4 nor 6 digits.</exception>
        /// <exception cref="MeteoSwissApiException">The request failed or its response could not be read.</exception>
        /// <exception cref="OperationCanceledException">The request was cancelled or timed out.</exception>
        Task<WeatherInfo> GetCurrentWeatherAsync(int plz, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Gets the weather forecast for the given postal code <paramref name="plz"/>.
        /// </summary>
        /// <param name="plz">
        ///     The postal code (PLZ) of the location: 4 digits such as 8001,
        ///     or 6 digits including the 2-digit suffix, such as 800100.
        /// </param>
        /// <param name="cancellationToken">Cancels the request.</param>
        /// <returns>The weather forecast info.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="plz"/> has neither 4 nor 6 digits.</exception>
        /// <exception cref="MeteoSwissApiException">The request failed or its response could not be read.</exception>
        /// <exception cref="OperationCanceledException">The request was cancelled or timed out.</exception>
        Task<ForecastInfo> GetForecastAsync(int plz, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Returns the weather icon for the given <paramref name="iconId"/> from <see cref="DefaultWeatherIconMapping"/>.
        ///     Icon ids without icon, such as the MeteoSwiss 'no data' icon id 32767, return a transparent 1x1 icon.
        /// </summary>
        /// <param name="iconId">The weather icon identifier.</param>
        /// <param name="cancellationToken">Cancels the request.</param>
        /// <returns>The weather icon as SVG stream. The caller disposes the stream.</returns>
        /// <exception cref="MeteoSwissApiException">The icon could not be downloaded.</exception>
        /// <exception cref="OperationCanceledException">The request was cancelled or timed out.</exception>
        Task<Stream> GetWeatherIconAsync(int iconId, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Returns the weather icon for the given <paramref name="iconId"/> from <paramref name="weatherIconMapping"/>.
        ///     Icon ids without icon, such as the MeteoSwiss 'no data' icon id 32767, return a transparent 1x1 icon.
        /// </summary>
        /// <param name="iconId">The weather icon identifier.</param>
        /// <param name="weatherIconMapping">The icon mapping to use, for example <see cref="HighContrastWeatherIconMapping"/>.</param>
        /// <param name="cancellationToken">Cancels the request.</param>
        /// <returns>The weather icon as SVG stream. The caller disposes the stream.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="weatherIconMapping"/> is null.</exception>
        /// <exception cref="MeteoSwissApiException">The icon could not be downloaded.</exception>
        /// <exception cref="OperationCanceledException">The request was cancelled or timed out.</exception>
        Task<Stream> GetWeatherIconAsync(int iconId, IWeatherIconMapping weatherIconMapping, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Returns the warning icon for the given <paramref name="warnLevel"/> from <see cref="DefaultWarningIconMapping"/>.
        ///     Warn levels without warning icon return a transparent 1x1 icon.
        /// </summary>
        /// <param name="warnLevel">The warning level.</param>
        /// <param name="cancellationToken">Cancels the request.</param>
        /// <returns>The warning icon as SVG stream. The caller disposes the stream.</returns>
        /// <exception cref="MeteoSwissApiException">The icon could not be downloaded.</exception>
        /// <exception cref="OperationCanceledException">The request was cancelled or timed out.</exception>
        Task<Stream> GetWarningIconAsync(WarnLevel warnLevel, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Returns the warning icon for the given <paramref name="warnLevel"/> from <paramref name="warningIconMapping"/>.
        ///     Warn levels without warning icon return a transparent 1x1 icon.
        /// </summary>
        /// <param name="warnLevel">The warning level.</param>
        /// <param name="warningIconMapping">The icon mapping to use, for example <see cref="HighContrastWarningIconMapping"/>.</param>
        /// <param name="cancellationToken">Cancels the request.</param>
        /// <returns>The warning icon as SVG stream. The caller disposes the stream.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="warningIconMapping"/> is null.</exception>
        /// <exception cref="MeteoSwissApiException">The icon could not be downloaded.</exception>
        /// <exception cref="OperationCanceledException">The request was cancelled or timed out.</exception>
        Task<Stream> GetWarningIconAsync(WarnLevel warnLevel, IWarningIconMapping warningIconMapping, CancellationToken cancellationToken = default);
    }
}