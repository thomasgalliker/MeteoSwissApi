namespace MeteoSwissApi
{
    public interface IWeatherIconMapping
    {
        /// <summary>
        ///     Returns the weather icon for the given <paramref name="iconId"/>.
        ///     The MeteoSwiss 'no data' icon id 32767 returns a transparent 1x1 icon.
        /// </summary>
        /// <param name="iconId">The weather icon identifier.</param>
        /// <returns>The weather icon stream.</returns>
        Task<Stream> GetIconAsync(int iconId);
    }
}