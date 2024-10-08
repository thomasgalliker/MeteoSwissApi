using UnitsNet.Units;

namespace MeteoSwissApi.Models.Converters
{
    internal class MinuteDurationJsonConverter : DurationJsonConverter
    {
        public MinuteDurationJsonConverter()
            : base(DurationUnit.Minute)
        {
        }
    }
}
