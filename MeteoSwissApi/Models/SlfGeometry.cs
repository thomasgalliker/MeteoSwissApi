using System.Collections.Generic;
using Newtonsoft.Json;

namespace MeteoSwissApi.Models
{
    public class SlfGeometry
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("coordinates")]
        public List<decimal> Coordinates { get; } = new List<decimal>();
    }
}
