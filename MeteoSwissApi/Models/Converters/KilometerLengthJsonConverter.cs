namespace MeteoSwissApi.Models.Converters
{
    internal sealed class KilometerLengthJsonConverter : LengthJsonConverter
    {
        public KilometerLengthJsonConverter()
            : base(LengthUnit.Kilometer)
        {

        }
    }
}
