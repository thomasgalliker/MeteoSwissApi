namespace MeteoSwissApi.Utils
{
    internal static class EmbeddedIcons
    {
        private static readonly Assembly Assembly = typeof(EmbeddedIcons).Assembly;

        /// <summary>
        /// The icons are embedded from Resources/Icons, so this is how their manifest resource names start.
        /// </summary>
        private const string ResourceNamePrefix = "MeteoSwissApi.Resources.Icons.";

        private const string TransparentIconResourceName = ResourceNamePrefix + "transparent.svg";

        internal static Stream GetTransparentIcon()
        {
            return Assembly.GetManifestResourceStream(TransparentIconResourceName)
                ?? throw new InvalidOperationException($"Embedded resource '{TransparentIconResourceName}' could not be loaded.");
        }

        /// <summary>
        /// Returns the embedded icon <paramref name="iconName"/>, such as "Weather.HighContrast.1.svg",
        /// or the transparent icon if there is no such icon.
        /// </summary>
        internal static Stream GetIconOrTransparentIcon(string iconName)
        {
            return Assembly.GetManifestResourceStream(ResourceNamePrefix + iconName) ?? GetTransparentIcon();
        }
    }
}