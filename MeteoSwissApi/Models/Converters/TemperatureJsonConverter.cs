using System.Text.Json;

namespace MeteoSwissApi.Models.Converters
{
    /// <summary>
    /// Reads/writes a <see cref="Temperature"/> using the MeteoSwiss wire format
    /// (a JSON number or an invariant-culture numeric string). Unlike
    /// <see cref="NullableTemperatureJsonConverter"/>, this converter does not treat the
    /// MeteoSwiss "no data" sentinel (32767) specially.
    /// </summary>
    internal class TemperatureJsonConverter : TemperatureJsonConverterBase<Temperature>
    {
        public override Temperature Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return TryReadDegreesCelsius(ref reader, out var celsius)
                ? Temperature.FromDegreesCelsius(celsius)
                : default;
        }

        public override void Write(Utf8JsonWriter writer, Temperature value, JsonSerializerOptions options)
        {
            WriteTemperature(writer, value);
        }
    }
}
