
namespace MeteoSwissApi.Models
{
    /// <summary>
    /// Latest values of an SLF station. A value is <c>null</c> if the station does not measure it
    /// or reported no value for it.
    /// </summary>
    public class SlfStationMeasurement
    {
        [JsonIgnore]
        public SlfStation Station { get; set; } = new SlfStation();

        /// <summary>
        /// New snow height of the last day, or <c>null</c> if the station reported no value.
        /// </summary>
        [JsonProperty("heightNewSnow")]
        public SlfStationDateLength? NewSnowHeight1d { get; set; }

        /// <summary>
        /// New snow height of the last three days, or <c>null</c> if the station reported no value.
        /// </summary>
        [JsonProperty("heightNewSnow3d")]
        public SlfStationDateLength? NewSnowHeight3d { get; set; }

        /// <summary>
        /// New snow height of the last seven days, or <c>null</c> if the station reported no value.
        /// </summary>
        [JsonProperty("heightNewSnow7d")]
        public SlfStationDateLength? NewSnowHeight7d { get; set; }

        /// <summary>
        /// Total snow height, or <c>null</c> if the station reported no value.
        /// </summary>
        [JsonProperty("snowHeight")]
        public SlfStationDateLength? SnowHeight { get; set; }

        /// <summary>
        /// Air temperature, or <c>null</c> if the station reported no value.
        /// </summary>
        [JsonProperty("temperatureAir")]
        public SlfStationDateTemperature? AirTemperature { get; set; }

        /// <summary>
        /// Snow surface temperature, or <c>null</c> if the station reported no value.
        /// </summary>
        [JsonProperty("temperatureSnowSurface")]
        public SlfStationDateTemperature? SurfaceTemperature { get; set; }

        /// <summary>
        /// The mean wind speed. A vectorial mean over a 30 minute measuring period,
        /// or <c>null</c> if the station reported no value.
        /// </summary>
        [JsonProperty("windVelocityMean")]
        public SlfStationDateSpeed? WindSpeedMean { get; set; }

        /// <summary>
        /// The wind direction: Direction of the vectorial mean,
        /// or <c>null</c> if the station reported no value.
        /// </summary>
        [JsonProperty("windDirectionMean")]
        public SlfStationDateAngle? WindDirection { get; set; }

        /// <summary>
        /// Maximum gust lasting for 5 seconds recorded during the 30-minute measuring period,
        /// or <c>null</c> if the station reported no value.
        /// </summary>
        /// <remarks>
        /// Only <see cref="ISlfDataService.GetLatestMeasurementByStationCodeAsync"/> returns this value.
        /// <see cref="ISlfDataService.GetLatestMeasurementsAsync"/> always returns <c>null</c>,
        /// because the SLF API offers no maximum wind for all stations at once.
        /// </remarks>
        [JsonProperty("windVelocityMax")]
        public SlfStationDateSpeed? WindSpeedMax { get; set; }

        public override string ToString()
        {
            return $"{this.Station?.Code}";
        }
    }
}