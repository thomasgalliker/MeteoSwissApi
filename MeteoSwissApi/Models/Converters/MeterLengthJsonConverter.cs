namespace MeteoSwissApi.Models.Converters
{
    internal sealed class MeterLengthJsonConverter : LengthJsonConverter
    {
        public MeterLengthJsonConverter()
            : base(LengthUnit.Meter)
        {
        }
    }
}
