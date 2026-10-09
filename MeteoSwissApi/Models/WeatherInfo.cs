namespace MeteoSwissApi.Models
{
    public class WeatherInfo
    {
        public WeatherInfo()
        {
            this.Forecast = Array.Empty<Forecast>();
            this.Warnings = Array.Empty<Warning>();
            this.WarningsOverview = Array.Empty<WarningsOverview>();
        }

        [JsonProperty("currentWeather")]
        public CurrentWeather CurrentWeather { get; set; } = null!;

        [JsonProperty("forecast")]
        [JsonConverter(typeof(EmptyArrayJsonConverter<Forecast>))]
        public Forecast[] Forecast { get; set; }

        [JsonProperty("warnings")]
        [JsonConverter(typeof(EmptyArrayJsonConverter<Warning>))]
        public Warning[] Warnings { get; set; }

        [JsonProperty("warningsOverview")]
        [JsonConverter(typeof(EmptyArrayJsonConverter<WarningsOverview>))]
        public WarningsOverview[] WarningsOverview { get; set; }

        [JsonProperty("graph")]
        public GraphDetail Graph { get; set; } = null!;

        [JsonProperty("klimaGraph")]
        public KlimaGraph KlimaGraph { get; set; } = null!;

        public override string ToString()
        {
            return $"Time: {this.CurrentWeather.Time}, Temperature: {this.CurrentWeather.Temperature}";
        }
    }
}
