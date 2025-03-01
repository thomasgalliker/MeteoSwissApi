using UnitsNet.Units;

namespace MeteoSwissApi.Models.Converters
{
    internal class HourDurationJsonConverter : DurationJsonConverter
    {
        public HourDurationJsonConverter()
            : base(DurationUnit.Hour)
        {
        }
    }
}
