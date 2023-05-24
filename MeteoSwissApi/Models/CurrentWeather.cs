using System;
using MeteoSwissApi.Models.Converters;
using Newtonsoft.Json;
using UnitsNet;

namespace MeteoSwissApi.Models
{
    public class CurrentWeather
    {
        [JsonProperty("time")]
        [JsonConverter(typeof(EpochDateTimeConverter))]
        public DateTime Time { get; set; }

        [JsonProperty("icon")]
        public int Icon { get; set; }

        [JsonProperty("iconV2")]
        public int IconV2 { get; set; }

        [JsonProperty("temperature")]
        [JsonConverter(typeof(TemperatureJsonConverter))]
        public Temperature Temperature { get; set; }
    }
}