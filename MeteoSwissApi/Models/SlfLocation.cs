using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MeteoSwissApi.Models
{
    [DebuggerDisplay("Lon: {this.Longitude} / Lat: {this.Latitude}")]
    public class SlfLocation
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("coordinates")]
        public List<decimal> Coordinates { get; set; } = new List<decimal>();

        [JsonIgnore]
        public decimal? Latitude { get { return this.Coordinates?.ElementAtOrDefault(1); } }

        [JsonIgnore]
        public decimal? Longitude { get { return this.Coordinates?.ElementAtOrDefault(0); } }
    }
}
