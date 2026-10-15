namespace MeteoSwissApi.Models.Converters
{
    /// <summary>
    /// Reads and writes a <see cref="Speed"/> in kilometers per hour, the unit of MeteoSwiss and SLF wind speeds.
    /// </summary>
    internal sealed class SpeedJsonConverter : QuantityJsonConverter<Speed>
    {
        public SpeedJsonConverter()
            : base(value => Speed.FromKilometersPerHour(value), speed => speed.KilometersPerHour)
        {
        }
    }
}
