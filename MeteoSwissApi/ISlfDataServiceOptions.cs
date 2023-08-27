namespace MeteoSwissApi
{
    public interface ISlfDataServiceOptions
    {
        /// <summary>
        /// The API endpoint.
        /// </summary>
        string ApiEndpoint { get; }

        /// <summary>
        /// Writes more verbose logging.
        /// </summary>
        bool VerboseLogging { get; set; }
    }
}
