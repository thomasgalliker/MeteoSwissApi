using System;
using Newtonsoft.Json;

namespace MeteoSwissApi.Models
{
    public class SlfProperties : SlfStation
    {
        [JsonProperty("value")]
        public decimal? Value { get; set; }

        [JsonProperty("velocity")]
        public decimal? Velocity { get; set; }

        [JsonProperty("direction")]
        public decimal? Direction { get; set; }

        [JsonProperty("timestamp")]
        public DateTime? Timestamp { get; set; }
    }
}
