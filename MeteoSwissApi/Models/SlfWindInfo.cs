namespace MeteoSwissApi.Models
{
    /// <summary>
    /// Wind values of a <see cref="SlfStationMeasurementItem"/>. SLF reports each value separately, so any of them
    /// is <c>null</c> if the station has no wind sensor or its sensor delivered no data.
    /// </summary>
    [DebuggerDisplay("Mean: {this.VelocityMean}, Max: {this.VelocityMax}, Direction: {this.Direction}")]
    public class SlfWindInfo
    {
        /// <summary>
        /// Maximum gust lasting for 5 seconds recorded during the 30-minute measuring period,
        /// or <c>null</c> if the station reported no value.
        /// </summary>
        public Speed? VelocityMax { get; set; }

        /// <summary>
        /// Mean wind speed (vectorial mean over the 30-minute measuring period),
        /// or <c>null</c> if the station reported no value.
        /// </summary>
        public Speed? VelocityMean { get; set; }

        /// <summary>
        /// Mean wind direction (direction of the vectorial mean),
        /// or <c>null</c> if the station reported no value.
        /// </summary>
        public Angle? Direction { get; set; }
    }
}
