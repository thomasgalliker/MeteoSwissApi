namespace MeteoSwissApi.Models
{
    [DebuggerDisplay("{this.Date}")]
    public class SlfStationMeasurementItem
    {
        /// <summary>
        /// The time of the measurement, in UTC.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Air temperature, or <c>null</c> if the station reported no value for <see cref="Date"/>.
        /// Observer stations, for example, report snow values but no air temperature.
        /// </summary>
        public Temperature? TemperatureAir { get; set; }

        public SlfWindInfo Wind { get; set; } = new SlfWindInfo();

        /// <summary>
        /// Total snow height, or <c>null</c> if the station reported no value for <see cref="Date"/>.
        /// </summary>
        public Length? SnowHeight { get; set; }

        /// <summary>
        /// New snow height. Reported on a coarser (daily) time grid than the other values,
        /// so this is <c>null</c> for most timestamps.
        /// </summary>
        public Length? NewSnowHeight { get; set; }

        /// <summary>
        /// Snow surface temperature, or <c>null</c> if the station reported no value for <see cref="Date"/>.
        /// </summary>
        public Temperature? SurfaceTemperature { get; set; }
    }
}