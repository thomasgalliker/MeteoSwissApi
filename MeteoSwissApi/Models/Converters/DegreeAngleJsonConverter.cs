namespace MeteoSwissApi.Models.Converters
{
    /// <summary>
    /// Reads and writes an <see cref="Angle"/> in degrees, the unit of MeteoSwiss and SLF wind directions and bearings.
    /// </summary>
    internal sealed class DegreeAngleJsonConverter : QuantityJsonConverter<Angle>
    {
        public DegreeAngleJsonConverter()
            : base(value => Angle.FromDegrees(value), angle => angle.Degrees)
        {
        }
    }
}
