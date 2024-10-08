using UnitsNet.Units;

namespace MeteoSwissApi.Models.Converters
{
    internal class PercentRatioJsonConverter : RatioJsonConverter
    {
        public PercentRatioJsonConverter()
            : base(RatioUnit.Percent)
        {
        }
    }
}
