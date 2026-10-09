namespace MeteoSwissApi
{
    /// <summary>
    /// Resolves warn levels to warning icons, for example by downloading them or from embedded resources.
    /// </summary>
    public interface IWarningIconMapping
    {
        /// <summary>
        ///     Returns the warning icon for the given <paramref name="warnLevel"/>.
        ///     Warn levels without warning icon, such as 0, 1 and levels MeteoSwiss added later, return a transparent 1x1 icon.
        /// </summary>
        /// <param name="warnLevel">The warning level.</param>
        /// <param name="cancellationToken">Cancels the request. Mappings that do not download icons may ignore it.</param>
        /// <returns>The warning icon as SVG stream. The caller disposes the stream.</returns>
        /// <exception cref="MeteoSwissApiException">The icon could not be downloaded. Only mappings that download icons throw it.</exception>
        /// <exception cref="OperationCanceledException">The request was cancelled or timed out.</exception>
        Task<Stream> GetIconAsync(WarnLevel warnLevel, CancellationToken cancellationToken = default);
    }
}