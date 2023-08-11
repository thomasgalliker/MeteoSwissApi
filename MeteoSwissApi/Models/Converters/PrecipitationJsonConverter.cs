using UnitsNet.Units;

namespace MeteoSwissApi.Models.Converters
{
    internal class PrecipitationJsonConverter : LengthJsonConverter
    {
        public PrecipitationJsonConverter()
            : base(LengthUnit.Millimeter)
        {

        }
    }
}
