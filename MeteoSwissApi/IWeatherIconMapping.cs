namespace MeteoSwissApi
{
    /// <summary>
    /// Resolves weather icon identifiers to icons, for example by downloading them or from embedded resources.
    /// </summary>
    public interface IWeatherIconMapping
    {
        /// <summary>
        ///     Returns the weather icon for the given <paramref name="iconId"/>.
        ///     Icon ids without icon, such as the MeteoSwiss 'no data' icon id 32767, return a transparent 1x1 icon.
        /// </summary>
        /// <param name="iconId">The weather icon identifier.</param>
        /// <param name="cancellationToken">Cancels the request. Mappings that do not download icons may ignore it.</param>
        /// <returns>The weather icon as SVG stream. The caller disposes the stream.</returns>
        /// <exception cref="MeteoSwissApiException">The icon could not be downloaded. Only mappings that download icons throw it.</exception>
        /// <exception cref="OperationCanceledException">The request was cancelled or timed out.</exception>
        Task<Stream> GetIconAsync(int iconId, CancellationToken cancellationToken = default);
    }
}