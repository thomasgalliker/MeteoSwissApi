using UnitsNet.Units;

namespace MeteoSwissApi.Models.Converters
{
    internal class KilometerLengthJsonConverter : LengthJsonConverter
    {
        public KilometerLengthJsonConverter()
            : base(LengthUnit.Kilometer)
        {

        }
    }
}
