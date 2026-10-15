namespace MeteoSwissApi.Models.Converters
{
    internal sealed class NullableMillimeterLengthJsonConverter : NullableLengthJsonConverter
    {
        public NullableMillimeterLengthJsonConverter()
            : base(LengthUnit.Millimeter)
        {
        }
    }
}
