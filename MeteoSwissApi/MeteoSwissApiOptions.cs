namespace MeteoSwissApi
{
    public class MeteoSwissApiOptions
    {
        /// <summary>
        /// The language to be used for service requests.
        /// Default: "en".
        /// </summary>
        public virtual string Language { get; set; } = "en";

        /// <summary>
        /// Writes more verbose logging.
        /// Default: <c>false</c>.
        /// </summary>
        public virtual bool VerboseLogging { get; set; } = false;

        public virtual SwissMetNetOptions SwissMetNet { get; set; } = new SwissMetNetOptions();
    }
}