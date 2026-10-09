namespace MeteoSwissApi
{
    public sealed class HighContrastWeatherIconMapping : IWeatherIconMapping
    {
        private const string IconNamePrefix = "Weather.HighContrast.";

        public Task<Stream> GetIconAsync(int iconId, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return Task.FromCanceled<Stream>(cancellationToken);
            }

            // Icon ids without high-contrast icon return the transparent icon.
            var iconName = FormattableString.Invariant($"{IconNamePrefix}{iconId}.svg");
            return Task.FromResult(EmbeddedIcons.GetIconOrTransparentIcon(iconName));
        }
    }
}