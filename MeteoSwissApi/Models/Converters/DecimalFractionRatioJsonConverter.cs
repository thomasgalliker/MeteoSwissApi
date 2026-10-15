namespace MeteoSwissApi.Models.Converters
{
    internal sealed class DecimalFractionRatioJsonConverter : RatioJsonConverter
    {
        public DecimalFractionRatioJsonConverter()
            : base(RatioUnit.DecimalFraction)
        {
        }
    }
}
