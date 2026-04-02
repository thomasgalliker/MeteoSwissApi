namespace MeteoSwissApi.Models
{
    internal class SlfStationMeasurementResponse
    {
        public SlfStationMeasurementResponse()
        {
            this.Features = Array.Empty<SlfFeature>();
        }

        [JsonProperty("type")]
        public string Type { get; set; } = null!;

        [JsonProperty("features")]
        public SlfFeature[] Features { get; set; }
    }
}
