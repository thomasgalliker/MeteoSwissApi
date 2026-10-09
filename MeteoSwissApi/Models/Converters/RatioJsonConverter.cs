namespace MeteoSwissApi.Models.Converters
{
    /// <summary>
    /// Reads and writes a <see cref="Ratio"/> in a fixed unit.
    /// </summary>
    internal abstract class RatioJsonConverter : QuantityJsonConverter<Ratio>
    {
        protected RatioJsonConverter(RatioUnit ratioUnit)
            : base(value => Ratio.From(value, ratioUnit), ratio => ratio.As(ratioUnit))
        {
        }
    }
}
