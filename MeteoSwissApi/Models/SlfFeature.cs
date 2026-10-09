
namespace MeteoSwissApi.Models
{
    internal sealed class SlfFeature
    {
        [JsonProperty("geometry")]
        public SlfGeometry Geometry { get; set; } = new SlfGeometry();

        [JsonProperty("properties")]
        public SlfProperties Properties { get; set; } = new SlfProperties();
    }
}