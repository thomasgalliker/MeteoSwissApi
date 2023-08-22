namespace MeteoSwissApi
{
    public class MeteoSwissWeatherServiceOptions : IMeteoSwissWeatherServiceOptions
    {
        public MeteoSwissWeatherServiceOptions()
        {
            this.ApiEndpoint = "https://app-prod-ws.meteoswiss-app.ch";
            this.Language = "en";
            this.VerboseLogging = false;
        }

        /// <summary>
        /// The API endpoint.
        /// </summary>
        public string ApiEndpoint { get; }

        /// <summary>
        /// The language to be used for service requests.
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Writes more verbose logging.
        /// </summary>
        public bool VerboseLogging { get; set; }
    }
}