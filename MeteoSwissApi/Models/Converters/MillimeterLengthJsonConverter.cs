using UnitsNet.Units;

namespace MeteoSwissApi.Models.Converters
{
    internal class MillimeterLengthJsonConverter : LengthJsonConverter
    {
        public MillimeterLengthJsonConverter()
            : base(LengthUnit.Millimeter)
        {

        }
    }
}
