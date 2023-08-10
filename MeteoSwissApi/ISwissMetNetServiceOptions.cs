namespace MeteoSwissApi
{
    public interface ISwissMetNetServiceOptions
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