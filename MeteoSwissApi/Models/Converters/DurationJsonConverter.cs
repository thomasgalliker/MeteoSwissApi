namespace MeteoSwissApi.Models.Converters
{
    /// <summary>
    /// Reads and writes a <see cref="Duration"/> in a fixed unit.
    /// </summary>
    internal abstract class DurationJsonConverter : QuantityJsonConverter<Duration>
    {
        protected DurationJsonConverter(DurationUnit durationUnit)
            : base(value => Duration.From(value, durationUnit), duration => duration.As(durationUnit))
        {
        }
    }
}
