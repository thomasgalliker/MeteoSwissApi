namespace MeteoSwissApi.Models.Converters
{
    internal sealed class PercentRatioJsonConverter : RatioJsonConverter
    {
        public PercentRatioJsonConverter()
            : base(RatioUnit.Percent)
        {
        }
    }
}
