using System.Collections.Generic;
using Newtonsoft.Json;

namespace MeteoSwissApi.Models
{
    internal class SlfStationMeasurementResponse
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("features")]
        public IReadOnlyCollection<SlfFeature> Features { get; } = new List<SlfFeature>();
    }
}
