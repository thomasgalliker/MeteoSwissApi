namespace MeteoSwissApi.Models
{
    internal sealed class SlfGeometry
    {
        [JsonProperty("type")]
        public string Type { get; set; } = null!;

        [JsonProperty("coordinates")]
        public List<double> Coordinates { get; set; } = new List<double>();
    }
}
