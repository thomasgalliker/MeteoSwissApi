namespace MeteoSwissApi
{
    public class MeteoSwissWeatherServiceConfiguration : IMeteoSwissWeatherServiceConfiguration
    {
        public MeteoSwissWeatherServiceConfiguration()
        {
            this.ApiEndpoint = "https://app-prod-ws.meteoswiss-app.ch";
            this.Language = "en";
            this.VerboseLogging = false;
        }

        public string ApiEndpoint { get; }

        public string Language { get; set; }

        public bool VerboseLogging { get; set; }
    }
}