namespace MeteoSwissApi.Models
{
    internal sealed class SlfStationMeasurementsResponse
    {
        public SlfStationMeasurementsResponse()
        {
            this.TemperatureAir = Array.Empty<SlfStationDateNullableTemperature>();
            this.WindVelocityMax = Array.Empty<SlfStationDateNullableSpeed>();
            this.WindVelocityMean = Array.Empty<SlfStationDateNullableSpeed>();
            this.WindDirectionMean = Array.Empty<SlfStationDateNullableAngle>();
            this.HeightNewSnow = Array.Empty<SlfStationDateNullableLength>();
            this.SnowHeight = Array.Empty<SlfStationDateNullableLength>();
            this.TemperatureSnowSurface = Array.Empty<SlfStationDateNullableTemperature>();
        }

        [JsonProperty("temperatureAir")]
        [JsonConverter(typeof(EmptyArrayJsonConverter<SlfStationDateNullableTemperature>))]
        public SlfStationDateNullableTemperature[] TemperatureAir { get; set; }

        /// <summary>
        /// New snow height. Note: this series uses a coarser (daily) time grid than the
        /// other series, so it only lines up with a subset of the measurement timestamps.
        /// </summary>
        [JsonProperty("heightNewSnow")]
        [JsonConverter(typeof(EmptyArrayJsonConverter<SlfStationDateNullableLength>))]
        public SlfStationDateNullableLength[] HeightNewSnow { get; set; }

        [JsonProperty("snowHeight")]
        [JsonConverter(typeof(EmptyArrayJsonConverter<SlfStationDateNullableLength>))]
        public SlfStationDateNullableLength[] SnowHeight { get; set; }

        [JsonProperty("temperatureSnowSurface")]
        [JsonConverter(typeof(EmptyArrayJsonConverter<SlfStationDateNullableTemperature>))]
        public SlfStationDateNullableTemperature[] TemperatureSnowSurface { get; set; }

        [JsonProperty("windVelocityMax")]
        [JsonConverter(typeof(EmptyArrayJsonConverter<SlfStationDateNullableSpeed>))]
        public SlfStationDateNullableSpeed[] WindVelocityMax { get; set; }

        [JsonProperty("windVelocityMean")]
        [JsonConverter(typeof(EmptyArrayJsonConverter<SlfStationDateNullableSpeed>))]
        public SlfStationDateNullableSpeed[] WindVelocityMean { get; set; }

        [JsonProperty("windDirectionMean")]
        [JsonConverter(typeof(EmptyArrayJsonConverter<SlfStationDateNullableAngle>))]
        public SlfStationDateNullableAngle[] WindDirectionMean { get; set; }
    }
}
