namespace MeteoSwissApi.Models
{
    /// <summary>
    /// Response of the SLF endpoint for the latest values of one station
    /// (public/station-data/timeseries/current/{network}/{code}).
    /// A parameter the station does not measure is missing, and a reported value can be <c>null</c>.
    /// </summary>
    internal sealed class SlfStationLatestMeasurementResponse
    {
        [JsonProperty("heightNewSnow")]
        public SlfStationDateNullableLength? HeightNewSnow { get; set; }

        [JsonProperty("heightNewSnow3d")]
        public SlfStationDateNullableLength? HeightNewSnow3d { get; set; }

        [JsonProperty("heightNewSnow7d")]
        public SlfStationDateNullableLength? HeightNewSnow7d { get; set; }

        [JsonProperty("snowHeight")]
        public SlfStationDateNullableLength? SnowHeight { get; set; }

        [JsonProperty("temperatureAir")]
        public SlfStationDateNullableTemperature? TemperatureAir { get; set; }

        [JsonProperty("temperatureSnowSurface")]
        public SlfStationDateNullableTemperature? TemperatureSnowSurface { get; set; }

        [JsonProperty("windVelocityMean")]
        public SlfStationDateNullableSpeed? WindVelocityMean { get; set; }

        [JsonProperty("windDirectionMean")]
        public SlfStationDateNullableAngle? WindDirectionMean { get; set; }

        [JsonProperty("windVelocityMax")]
        public SlfStationDateNullableSpeed? WindVelocityMax { get; set; }
    }
}
