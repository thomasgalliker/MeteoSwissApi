namespace MeteoSwissApi.Models
{
    internal sealed class SlfStationMeasurementResponse
    {
        public SlfStationMeasurementResponse()
        {
            this.Features = Array.Empty<SlfFeature>();
        }

        [JsonProperty("type")]
        public string Type { get; set; } = null!;

        [JsonProperty("features")]
        [JsonConverter(typeof(EmptyArrayJsonConverter<SlfFeature>))]
        public SlfFeature[] Features { get; set; }
    }
}
