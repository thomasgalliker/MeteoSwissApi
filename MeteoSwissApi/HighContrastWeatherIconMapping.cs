namespace MeteoSwissApi
{
    public class HighContrastWeatherIconMapping : IWeatherIconMapping
    {
        private static readonly Assembly Assembly = typeof(HighContrastWeatherIconMapping).Assembly;
        private const string EmbeddedResourcePath = "Icons.Weather.HighContrast.{0}.svg";

        /// <summary>
        /// MeteoSwiss encodes missing/unavailable values as 32767 (0x7FFF) in the JSON API.
        /// There is no icon for this id; an embedded transparent 1x1 icon is returned instead.
        /// </summary>
        private const int NoDataIconId = 32767;

        public HighContrastWeatherIconMapping()
        {
        }

        public Task<Stream> GetIconAsync(int iconId)
        {
            if (iconId == NoDataIconId)
            {
                return Task.FromResult(EmbeddedIcons.GetTransparentIcon());
            }

            var resourceFileName = string.Format(EmbeddedResourcePath, iconId);
            var stream = ResourceLoader.Current.GetEmbeddedResourceStream(Assembly, resourceFileName);
            return Task.FromResult(stream);
        }
    }
}