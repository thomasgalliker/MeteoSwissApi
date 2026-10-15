namespace MeteoSwissApi
{
    public sealed class HighContrastWarningIconMapping : IWarningIconMapping
    {
        private const string IconNamePrefix = "Warning.HighContrast.";

        public Task<Stream> GetIconAsync(WarnLevel warnLevel, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return Task.FromCanceled<Stream>(cancellationToken);
            }

            if (warnLevel == WarnLevel.NoWarnLevel ||
                warnLevel == WarnLevel.Level1)
            {
                // MeteoSwiss does not provide warning icons below warn level 2
                return Task.FromResult(EmbeddedIcons.GetTransparentIcon());
            }

            // Warn levels without high-contrast icon, such as levels MeteoSwiss added later, return the transparent icon.
            var iconName = FormattableString.Invariant($"{IconNamePrefix}{warnLevel.Level}.svg");
            return Task.FromResult(EmbeddedIcons.GetIconOrTransparentIcon(iconName));
        }
    }
}