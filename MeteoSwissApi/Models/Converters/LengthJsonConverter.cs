namespace MeteoSwissApi.Models.Converters
{
    /// <summary>
    /// Reads and writes a <see cref="Length"/> in a fixed unit.
    /// </summary>
    internal abstract class LengthJsonConverter : QuantityJsonConverter<Length>
    {
        protected LengthJsonConverter(LengthUnit lengthUnit)
            : base(value => Length.From(value, lengthUnit), length => length.As(lengthUnit))
        {
        }
    }
}
