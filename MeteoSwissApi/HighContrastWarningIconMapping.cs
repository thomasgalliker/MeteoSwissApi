namespace MeteoSwissApi
{
    public class HighContrastWarningIconMapping : IWarningIconMapping
    {
        private static readonly Assembly Assembly = typeof(HighContrastWarningIconMapping).Assembly;
        private const string EmbeddedResourcePath = "Icons.Warning.HighContrast.{0}.svg";

        public HighContrastWarningIconMapping()
        {
        }

        public Task<Stream> GetIconAsync(WarnLevel warnLevel)
        {
            if (warnLevel == WarnLevel.NoWarnLevel ||
                warnLevel == WarnLevel.Level1)
            {
                // MeteoSwiss does not provide warning icons below warn level 2
                return Task.FromResult(EmbeddedIcons.GetTransparentIcon());
            }

            var resourceFileName = string.Format(EmbeddedResourcePath, warnLevel.Level);
            var stream = ResourceLoader.Current.GetEmbeddedResourceStream(Assembly, resourceFileName);
            return Task.FromResult(stream);
        }
    }
}