namespace MeteoSwissApi.Models.Converters
{
    internal sealed class MillimeterLengthJsonConverter : LengthJsonConverter
    {
        public MillimeterLengthJsonConverter()
            : base(LengthUnit.Millimeter)
        {

        }
    }
}
