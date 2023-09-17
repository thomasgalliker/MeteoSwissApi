using Newtonsoft.Json;

namespace MeteoSwissApi.Models
{
    public class SlfStationMeasurement
    {
        [JsonIgnore]
        public SlfStation Station { get; set; }

        [JsonProperty("heightNewSnow")]
        public SlfStationDateLength NewSnowHeight1d { get; set; }

        [JsonProperty("heightNewSnow3d")]
        public SlfStationDateLength NewSnowHeight3d { get; set; }

        [JsonProperty("heightNewSnow7d")]
        public SlfStationDateLength NewSnowHeight7d { get; set; }

        [JsonProperty("snowHeight")]
        public SlfStationDateLength SnowHeight { get; set; }

        [JsonProperty("temperatureAir")]
        public SlfStationDateTemperature AirTemperature { get; set; }

        [JsonProperty("temperatureSnowSurface")]
        public SlfStationDateTemperature SurfaceTemperature { get; set; }

        /// <summary>
        /// The mean wind speed. A vectorial mean over a 30 minute measuring period.
        /// </summary>
        [JsonProperty("windVelocityMean")]
        public SlfStationDateSpeed WindSpeedMean { get; set; }

        /// <summary>
        /// The wind direction: Direction of the vectorial mean.
        /// </summary>
        [JsonProperty("windDirectionMean")]
        public SlfStationDateAngle WindDirection { get; set; }

        /// <summary>
        /// Maximum gust lasting for 5 seconds recorded during the 30-minute measuring period.
        /// </summary>
        [JsonProperty("windVelocityMax")]
        public SlfStationDateSpeed WindSpeedMax { get; set; }

        public override string ToString()
        {
            return $"{this.Station?.Code}";
        }
    }
}