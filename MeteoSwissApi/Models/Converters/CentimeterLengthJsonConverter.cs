using UnitsNet.Units;

namespace MeteoSwissApi.Models.Converters
{
    internal class CentimeterLengthJsonConverter : LengthJsonConverter
    {
        public CentimeterLengthJsonConverter()
            : base(LengthUnit.Centimeter)
        {
        }
    }
}
