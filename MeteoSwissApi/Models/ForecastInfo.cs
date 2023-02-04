using System.Collections.Generic;
using Newtonsoft.Json;

namespace MeteoSwissApi.Models
{
    public class ForecastInfo
    {
        public ForecastInfo()
        {
            this.Forecast = new List<Forecast>();
            this.WarningsOverview = new List<WarningsOverview>();
        }

        [JsonProperty("plz")]
        public int Plz { get; set; }

        [JsonProperty("currentWeather")]
        public CurrentWeather CurrentWeather { get; set; }

        [JsonProperty("regionForecast")]
        public IReadOnlyCollection<Forecast> Forecast { get; set; }

        [JsonProperty("graph")]
        public GraphCompact Graph { get; set; }

        [JsonProperty("warningsOverview")]
        public IReadOnlyCollection<WarningsOverview> WarningsOverview { get; set; }

        public override string ToString()
        {
            return $"Plz={this.Plz}, Forecast={{{this.Forecast.Count}}}";
        }
    }
}
