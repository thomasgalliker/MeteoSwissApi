using System.Collections.Generic;
using Newtonsoft.Json;

namespace MeteoSwissApi.Models
{
    public class WeatherInfo
    {
        public WeatherInfo()
        {
            this.Warnings = new List<Warning>();
            this.WarningsOverview = new List<WarningsOverview>();
        }

        [JsonProperty("currentWeather")]
        public CurrentWeather CurrentWeather { get; set; }

        [JsonProperty("forecast")]
        public List<Forecast> Forecast { get; set; }

        [JsonProperty("warnings")]
        public List<Warning> Warnings { get; set; }

        [JsonProperty("warningsOverview")]
        public List<WarningsOverview> WarningsOverview { get; set; }

        [JsonProperty("graph")]
        public Graph Graph { get; set; }
    }
}