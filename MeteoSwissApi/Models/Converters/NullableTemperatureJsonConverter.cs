using System.Text.Json;
using System.Text.Json.Serialization;

namespace MeteoSwissApi.Models.Converters
{
    /// <summary>
    /// Reads a temperature in degrees Celsius from a JSON number or an invariant-culture numeric string and maps
    /// missing values (JSON <c>null</c>, unreadable values and the MeteoSwiss "no data" value 32767) to <c>null</c>.
    /// </summary>
    internal sealed class NullableTemperatureJsonConverter : JsonConverter<Temperature?>
    {
        /// <summary>
        /// MeteoSwiss encodes missing/unavailable measurements as 32767 (0x7FFF)
        /// in the JSON API (e.g. a frozen <c>currentWeather</c> block reports <c>temperature: 32767</c>).
        /// </summary>
        private const double NoDataValue = 32767d;

        public override Temperature? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (JsonNumberReader.TryReadDouble(ref reader, out var celsius) && Math.Abs(celsius) < NoDataValue)
            {
                return Temperature.FromDegreesCelsius(celsius);
            }

            return null;
        }

        public override void Write(Utf8JsonWriter writer, Temperature? value, JsonSerializerOptions options)
        {
            if (value is Temperature temperature)
            {
                writer.WriteNumberValue(temperature.DegreesCelsius);
            }
            else
            {
                writer.WriteNullValue();
            }
        }
    }
}
