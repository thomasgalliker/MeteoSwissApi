namespace MeteoSwissApi.Models.Converters
{
    internal sealed class CentimeterLengthJsonConverter : LengthJsonConverter
    {
        public CentimeterLengthJsonConverter()
            : base(LengthUnit.Centimeter)
        {
        }
    }
}
