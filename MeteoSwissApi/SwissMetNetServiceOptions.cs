namespace MeteoSwissApi
{
    public class SwissMetNetServiceOptions : ISwissMetNetServiceOptions
    {
        public SwissMetNetServiceOptions()
        {
            this.ApiEndpoint = "https://data.geo.admin.ch";
            this.VerboseLogging = false;
        }

        public string ApiEndpoint { get; }

        public bool VerboseLogging { get; set; }
    }
}