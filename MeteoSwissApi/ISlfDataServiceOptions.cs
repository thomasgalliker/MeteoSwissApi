namespace MeteoSwissApi
{
    public interface ISlfDataServiceOptions
    {
        /// <summary>
        /// The SLF API endpoint.
        /// </summary>
        string SlfApiEndpoint { get; }
        
        /// <summary>
        /// The SLF API endpoint.
        /// </summary>
        string WhiteRiskApiEndpoint { get; }

        /// <summary>
        /// Writes more verbose logging.
        /// </summary>
        bool VerboseLogging { get; set; }
    }
}
