namespace MeteoSwissApi.Models
{
    internal class SlfStationMeasurementsResponse
    {
        public SlfStationMeasurementsResponse()
        {
            this.TemperatureAir = Array.Empty<SlfStationDateTemperature>();
            this.WindVelocityMax = Array.Empty<SlfStationDateSpeed>();
            this.WindVelocityMean = Array.Empty<SlfStationDateSpeed>();
            this.WindDirectionMean = Array.Empty<SlfStationDateAngle>();
        }

        [JsonProperty("temperatureAir")]
        public SlfStationDateTemperature[] TemperatureAir { get; set; }

        [JsonProperty("windVelocityMax")]
        public SlfStationDateSpeed[] WindVelocityMax { get; set; }

        [JsonProperty("windVelocityMean")]
        public SlfStationDateSpeed[] WindVelocityMean { get; set; }

        [JsonProperty("windDirectionMean")]
        public SlfStationDateAngle[] WindDirectionMean { get; set; }
    }
}
