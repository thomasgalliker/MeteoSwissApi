namespace MeteoSwissApi.Models
{
    public class ForecastInfo
    {
        public ForecastInfo()
        {
            this.Forecast = Array.Empty<Forecast>();
            this.WarningsOverview = Array.Empty<WarningsOverview>();
        }

        [JsonProperty("plz")]
        public int Plz { get; set; }

        [JsonProperty("currentWeather")]
        public CurrentWeather CurrentWeather { get; set; } = null!;

        [JsonProperty("regionForecast")]
        public Forecast[] Forecast { get; set; }

        [JsonProperty("graph")]
        public GraphCompact Graph { get; set; } = null!;

        [JsonProperty("warningsOverview")]
        public WarningsOverview[] WarningsOverview { get; set; }

        public override string ToString()
        {
            return $"Plz={this.Plz}, Forecast={{{this.Forecast.Length}}}";
        }
    }
}

