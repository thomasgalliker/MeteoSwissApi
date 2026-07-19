using System.Text.Json;
using System.Text.Json.Serialization;

namespace MeteoSwissApi.Models.Converters
{
    /// <summary>
    /// Shared base for the <see cref="Temperature"/> JSON converters. It centralizes the
    /// MeteoSwiss wire format — a temperature is read from either a JSON number or an
    /// invariant-culture numeric string and written back as an invariant-culture string —
    /// and leaves the null/sentinel policy to the derived converter.
    /// </summary>
    /// <typeparam name="T"><see cref="Temperature"/> or <see cref="Nullable{Temperature}"/>.</typeparam>
    internal abstract class TemperatureJsonConverterBase<T> : JsonConverter<T>
    {
        /// <summary>
        /// Reads the current token as a temperature in degrees Celsius, accepting both a JSON
        /// number and an invariant-culture numeric string. Returns <c>false</c> for any other token.
        /// </summary>
        protected static bool TryReadDegreesCelsius(ref Utf8JsonReader reader, out double celsius)
        {
            if (reader.TokenType == JsonTokenType.Number)
            {
                celsius = reader.GetDouble();
                return true;
            }

            if (reader.TokenType == JsonTokenType.String
                && double.TryParse(reader.GetString(), NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out celsius))
            {
                return true;
            }

            celsius = default;
            return false;
        }

        /// <summary>
        /// Writes a <see cref="Temperature"/> as an invariant-culture string, matching the format
        /// expected by the MeteoSwiss API.
        /// </summary>
        protected static void WriteTemperature(Utf8JsonWriter writer, Temperature temperature)
        {
            writer.WriteStringValue(temperature.Value.ToString(CultureInfo.InvariantCulture));
        }
    }
}
