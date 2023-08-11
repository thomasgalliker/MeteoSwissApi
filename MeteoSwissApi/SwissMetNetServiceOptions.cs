using System;

namespace MeteoSwissApi
{
    public class SwissMetNetServiceOptions : ISwissMetNetServiceOptions
    {
        public SwissMetNetServiceOptions()
        {
            this.ApiEndpoint = "https://data.geo.admin.ch";
            this.VerboseLogging = false;
            this.CacheExpiration = TimeSpan.FromMinutes(20);
        }

        public string ApiEndpoint { get; }

        public bool VerboseLogging { get; set; }

        public TimeSpan CacheExpiration { get; set; }
    }
}