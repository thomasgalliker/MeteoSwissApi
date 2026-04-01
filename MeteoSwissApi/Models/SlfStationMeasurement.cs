
namespace MeteoSwissApi.Models
{
    public class SlfStationMeasurement
    {
        [JsonIgnore]
        public SlfStation Station { get; set; } = new SlfStation();

        [JsonProperty("heightNewSnow")]
        public SlfStationDateLength NewSnowHeight1d { get; set; } = new SlfStationDateLength();

        [JsonProperty("heightNewSnow3d")]
        public SlfStationDateLength NewSnowHeight3d { get; set; } = new SlfStationDateLength();

        [JsonProperty("heightNewSnow7d")]
        public SlfStationDateLength NewSnowHeight7d { get; set; } = new SlfStationDateLength();

        [JsonProperty("snowHeight")]
        public SlfStationDateLength SnowHeight { get; set; } = new SlfStationDateLength();

        [JsonProperty("temperatureAir")]
        public SlfStationDateTemperature AirTemperature { get; set; } = new SlfStationDateTemperature();

        [JsonProperty("temperatureSnowSurface")]
        public SlfStationDateTemperature SurfaceTemperature { get; set; } = new SlfStationDateTemperature();

        /// <summary>
        /// The mean wind speed. A vectorial mean over a 30 minute measuring period.
        /// </summary>
        [JsonProperty("windVelocityMean")]
        public SlfStationDateSpeed WindSpeedMean { get; set; } = new SlfStationDateSpeed();

        /// <summary>
        /// The wind direction: Direction of the vectorial mean.
        /// </summary>
        [JsonProperty("windDirectionMean")]
        public SlfStationDateAngle WindDirection { get; set; } = new SlfStationDateAngle();

        /// <summary>
        /// Maximum gust lasting for 5 seconds recorded during the 30-minute measuring period.
        /// </summary>
        [JsonProperty("windVelocityMax")]
        public SlfStationDateSpeed WindSpeedMax { get; set; } = new SlfStationDateSpeed();

        public override string ToString()
        {
            return $"{this.Station?.Code}";
        }
    }
}