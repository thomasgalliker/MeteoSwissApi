namespace MeteoSwissApi
{
    public interface IWarningIconMapping
    {
        /// <summary>
        ///     Returns the warning icon for the given <paramref name="warnLevel"/>.
        ///     Warn levels without warning icon (0 and 1) return a transparent 1x1 icon.
        /// </summary>
        /// <param name="warnLevel">The warning level.</param>
        /// <returns>The warning icon stream.</returns>
        Task<Stream> GetIconAsync(WarnLevel warnLevel);
    }
}