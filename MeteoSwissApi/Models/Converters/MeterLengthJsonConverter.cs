using UnitsNet.Units;

namespace MeteoSwissApi.Models.Converters
{
    internal class MeterLengthJsonConverter : LengthJsonConverter
    {
        public MeterLengthJsonConverter()
            : base(LengthUnit.Meter)
        {
        }
    }
}
