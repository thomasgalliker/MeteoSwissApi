namespace MeteoSwissApi.Utils
{
    internal static class EmbeddedIcons
    {
        private static readonly Assembly Assembly = typeof(EmbeddedIcons).Assembly;

        private const string TransparentIconResourceFileName = "transparent.svg";

        internal static Stream GetTransparentIcon()
        {
            return ResourceLoader.Current.GetEmbeddedResourceStream(Assembly, TransparentIconResourceFileName);
        }
    }
}