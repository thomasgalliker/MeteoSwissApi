namespace MeteoSwissApi
{
    public interface IMeteoSwissWeatherService
    {
        /// <summary>
        ///     Gets the current weather for the given postal code <paramref name="plz"/>.
        /// </summary>
        /// <param name="plz">The postal code (PLZ) of the location.</param>
        /// <returns>The current weather info.</returns>
        Task<WeatherInfo> GetCurrentWeatherAsync(int plz);

        /// <summary>
        ///     Gets the weather forecast for the given postal code <paramref name="plz"/>.
        /// </summary>
        /// <param name="plz">The postal code (PLZ) of the location.</param>
        /// <returns>The weather forecast info.</returns>
        Task<ForecastInfo> GetForecastAsync(int plz);

        /// <summary>
        ///     Returns the weather icon for the given <paramref name="iconId"/>.
        ///     The MeteoSwiss 'no data' icon id 32767 returns a transparent 1x1 icon.
        /// </summary>
        /// <param name="iconId">The weather icon identifier.</param>
        /// <param name="weatherIconMapping">The icon mapping to use. Default: <see cref="DefaultWeatherIconMapping"/>.</param>
        /// <returns>The weather icon stream.</returns>
        Task<Stream> GetWeatherIconAsync(int iconId, IWeatherIconMapping? weatherIconMapping = null);

        /// <summary>
        ///     Returns the warning icon for the given <paramref name="warnLevel"/>.
        /// </summary>
        /// <param name="warnLevel">The warning level.</param>
        /// <param name="warningIconMapping">The icon mapping to use. Default: <see cref="DefaultWarningIconMapping"/>.</param>
        /// <returns>The warning icon stream.</returns>
        Task<Stream> GetWarningIconAsync(WarnLevel warnLevel, IWarningIconMapping? warningIconMapping = null);
    }
}