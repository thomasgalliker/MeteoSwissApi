using UnitsNet.Units;

namespace MeteoSwissApi.Models.Converters
{
    internal class DecimalFractionRatioJsonConverter : RatioJsonConverter
    {
        public DecimalFractionRatioJsonConverter()
            : base(RatioUnit.DecimalFraction)
        {
        }
    }
}
