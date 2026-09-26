namespace MeteoSwissApi.Models
{
    internal class SlfStationMeasurementsResponse
    {
        public SlfStationMeasurementsResponse()
        {
            this.TemperatureAir = Array.Empty<SlfStationDateTemperature>();
            this.WindVelocityMax = Array.Empty<SlfStationDateNullableSpeed>();
            this.WindVelocityMean = Array.Empty<SlfStationDateNullableSpeed>();
            this.WindDirectionMean = Array.Empty<SlfStationDateNullableAngle>();
            this.HeightNewSnow = Array.Empty<SlfStationDateLength>();
            this.SnowHeight = Array.Empty<SlfStationDateLength>();
            this.TemperatureSnowSurface = Array.Empty<SlfStationDateTemperature>();
        }

        [JsonProperty("temperatureAir")]
        public SlfStationDateTemperature[] TemperatureAir { get; set; }

        /// <summary>
        /// New snow height. Note: this series uses a coarser (daily) time grid than the
        /// other series, so it only lines up with a subset of the measurement timestamps.
        /// </summary>
        [JsonProperty("heightNewSnow")]
        public SlfStationDateLength[] HeightNewSnow { get; set; }

        [JsonProperty("snowHeight")]
        public SlfStationDateLength[] SnowHeight { get; set; }

        [JsonProperty("temperatureSnowSurface")]
        public SlfStationDateTemperature[] TemperatureSnowSurface { get; set; }

        [JsonProperty("windVelocityMax")]
        public SlfStationDateNullableSpeed[] WindVelocityMax { get; set; }

        [JsonProperty("windVelocityMean")]
        public SlfStationDateNullableSpeed[] WindVelocityMean { get; set; }

        [JsonProperty("windDirectionMean")]
        public SlfStationDateNullableAngle[] WindDirectionMean { get; set; }
    }
}
