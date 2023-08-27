namespace MeteoSwissApi
{
    public class SlfDataServiceOptions : ISlfDataServiceOptions
    {
        public SlfDataServiceOptions()
        {
            this.SlfApiEndpoint = "https://public-meas-data-v2.slf.ch";
            this.WhiteRiskApiEndpoint = "https://whiterisk.ch";
            this.VerboseLogging = false;
        }

        /// <inheritdoc />
        public string SlfApiEndpoint { get; }
        
        /// <inheritdoc />
        public string WhiteRiskApiEndpoint { get; }

        /// <inheritdoc />
        public bool VerboseLogging { get; set; }
    }
}
