namespace MeteoSwissApi.Models.Converters
{
    /// <summary>
    /// Reads and writes a <see cref="TemperatureDelta"/> in degrees Celsius.
    /// </summary>
    internal sealed class TemperatureDeltaJsonConverter : QuantityJsonConverter<TemperatureDelta>
    {
        public TemperatureDeltaJsonConverter()
            : base(value => TemperatureDelta.FromDegreesCelsius(value), temperatureDelta => temperatureDelta.DegreesCelsius)
        {
        }
    }
}
