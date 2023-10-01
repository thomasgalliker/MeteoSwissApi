using Newtonsoft.Json;

namespace MeteoSwissApi.Models
{
    internal class SlfFeature
    {
        [JsonProperty("geometry")]
        public SlfGeometry Geometry { get; set; }

        [JsonProperty("properties")]
        public SlfProperties Properties { get; set; }
    }
}
