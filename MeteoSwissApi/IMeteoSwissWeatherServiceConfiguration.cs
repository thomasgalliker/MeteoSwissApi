namespace MeteoSwissApi
{
    public interface IMeteoSwissWeatherServiceConfiguration
    {
        /// <summary>
        /// The API endpoint.
        /// </summary>
        string ApiEndpoint { get; }

        /// <summary>
        /// Language used for API requests.
        /// Select one of: de, en, it, fr
        /// </summary>
        string Language { get; set; }

        /// <summary>
        /// Writes more verbose logging.
        /// </summary>
        bool VerboseLogging { get; set; }
    }
}