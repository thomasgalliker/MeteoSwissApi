namespace MeteoSwissApi.Models
{
    internal class SlfGeometry
    {
        [JsonProperty("type")]
        public string Type { get; set; } = null!;

        [JsonProperty("coordinates")]
        public List<double> Coordinates { get; } = new List<double>();
    }
}
