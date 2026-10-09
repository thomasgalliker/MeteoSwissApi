namespace MeteoSwissApi.Models.Converters
{
    internal sealed class MinuteDurationJsonConverter : DurationJsonConverter
    {
        public MinuteDurationJsonConverter()
            : base(DurationUnit.Minute)
        {
        }
    }
}
