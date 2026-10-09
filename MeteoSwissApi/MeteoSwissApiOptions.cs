namespace MeteoSwissApi
{
    public class MeteoSwissApiOptions
    {
        /// <summary>
        /// The languages the MeteoSwiss API serves: de, fr, it and en.
        /// </summary>
        public static readonly IReadOnlyList<string> SupportedLanguages = Array.AsReadOnly(new[] { "de", "fr", "it", "en" });

        /// <summary>
        /// The language of the texts the MeteoSwiss API returns, one of <see cref="SupportedLanguages"/>.
        /// Default: "en".
        /// </summary>
        public virtual string Language { get; set; } = "en";

        /// <summary>
        /// Writes the content of every API response to the log at debug level.
        /// Default: <c>false</c>.
        /// </summary>
        public virtual bool VerboseLogging { get; set; } = false;

        /// <summary>
        /// The options of <see cref="SwissMetNetService"/>.
        /// </summary>
        public virtual SwissMetNetOptions SwissMetNet { get; set; } = new SwissMetNetOptions();
    }
}