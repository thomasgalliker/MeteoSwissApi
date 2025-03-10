﻿using System.Collections.Generic;
using Newtonsoft.Json;

namespace MeteoSwissApi.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public class WeatherInfo
    {
        public WeatherInfo()
        {
            this.Forecast = new List<Forecast>();
            this.Warnings = new List<Warning>();
            this.WarningsOverview = new List<WarningsOverview>();
        }

        [JsonProperty("currentWeather")]
        public CurrentWeather CurrentWeather { get; set; }

        [JsonProperty("forecast")]
        public IReadOnlyCollection<Forecast> Forecast { get; set; }

        [JsonProperty("warnings")]
        public IReadOnlyCollection<Warning> Warnings { get; set; }

        [JsonProperty("warningsOverview")]
        public IReadOnlyCollection<WarningsOverview> WarningsOverview { get; set; }

        [JsonProperty("graph")]
        public GraphDetail Graph { get; set; }

        [JsonProperty("klimaGraph")]
        public KlimaGraph KlimaGraph { get; set; }

        public override string ToString()
        {
            return $"Time: {this.CurrentWeather.Time}, Temperature: {this.CurrentWeather.Temperature}, ";
        }
    }
}