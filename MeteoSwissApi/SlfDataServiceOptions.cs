namespace MeteoSwissApi
{
    public class SlfDataServiceOptions : ISlfDataServiceOptions
    {
        public SlfDataServiceOptions()
        {
            this.ApiEndpoint = "https://public-meas-data-v2.slf.ch";
            this.VerboseLogging = false;
        }

        /// <inheritdoc />
        public string ApiEndpoint { get; }

        /// <inheritdoc />
        public bool VerboseLogging { get; set; }
    }
}
