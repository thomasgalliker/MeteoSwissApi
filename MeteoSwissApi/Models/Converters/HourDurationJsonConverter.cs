namespace MeteoSwissApi.Models.Converters
{
    internal sealed class HourDurationJsonConverter : DurationJsonConverter
    {
        public HourDurationJsonConverter()
            : base(DurationUnit.Hour)
        {
        }
    }
}
