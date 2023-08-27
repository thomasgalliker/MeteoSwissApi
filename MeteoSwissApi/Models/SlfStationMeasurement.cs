using Newtonsoft.Json;

namespace MeteoSwissApi.Models
{
    public class SlfStationMeasurement
    {
        [JsonIgnore]
        public SlfStation Station { get; set; }

        [JsonProperty("heightNewSnow")]
        public SlfStationMeasurementLengthValue NewSnowHeight1d { get; set; }

        [JsonProperty("heightNewSnow3d")]
        public SlfStationMeasurementLengthValue NewSnowHeight3d { get; set; }

        [JsonProperty("heightNewSnow7d")]
        public SlfStationMeasurementLengthValue NewSnowHeight7d { get; set; }

        [JsonProperty("snowHeight")]
        public SlfStationMeasurementLengthValue SnowHeight { get; set; }

        [JsonProperty("temperatureAir")]
        public SlfStationMeasurementTemperatureValue AirTemperature { get; set; }

        [JsonProperty("temperatureSnowSurface")]
        public SlfStationMeasurementTemperatureValue SurfaceTemperature { get; set; }

        /// <summary>
        /// The mean wind speed. A vectorial mean over a 30 minute measuring period.
        /// </summary>
        [JsonProperty("windVelocityMean")]
        public SlfStationMeasurementSpeedValue WindSpeedMean { get; set; }

        /// <summary>
        /// The wind direction: Direction of the vectorial mean.
        /// </summary>
        [JsonProperty("windDirectionMean")]
        public SlfStationMeasurementAngleValue WindDirection { get; set; }

        /// <summary>
        /// Maximum gust lasting for 5 seconds recorded during the 30-minute measuring period.
        /// </summary>
        [JsonProperty("windVelocityMax")]
        public SlfStationMeasurementSpeedValue WindSpeedMax { get; set; }

        public override string ToString()
        {
            return $"{this.Station?.Code}";
        }
    }
}