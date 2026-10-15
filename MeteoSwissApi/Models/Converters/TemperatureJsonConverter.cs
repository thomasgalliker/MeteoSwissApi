namespace MeteoSwissApi.Models.Converters
{
    /// <summary>
    /// Reads and writes a <see cref="Temperature"/> in degrees Celsius, the unit of the MeteoSwiss and SLF APIs.
    /// Unlike <see cref="NullableTemperatureJsonConverter"/>, this converter does not treat the
    /// MeteoSwiss "no data" sentinel (32767) specially.
    /// </summary>
    internal sealed class TemperatureJsonConverter : QuantityJsonConverter<Temperature>
    {
        public TemperatureJsonConverter()
            : base(value => Temperature.FromDegreesCelsius(value), temperature => temperature.DegreesCelsius)
        {
        }
    }
}
