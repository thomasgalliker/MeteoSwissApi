using System.Text.Json;

namespace MeteoSwissApi.Models.Converters
{
    internal static class JsonNumberReader
    {
        /// <summary>
        /// Reads the current token as a number, accepting both a JSON number and an invariant-culture numeric string
        /// such as <c>"14.6"</c>. Returns <c>false</c> for any other token, and for <c>"NaN"</c> and the infinities,
        /// which no quantity can hold.
        /// </summary>
        internal static bool TryReadDouble(ref Utf8JsonReader reader, out double value)
        {
            if (reader.TokenType == JsonTokenType.Number)
            {
                value = reader.GetDouble();
                return true;
            }

            if (reader.TokenType == JsonTokenType.String
                && double.TryParse(reader.GetString(), NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out value)
                && !double.IsNaN(value)
                && !double.IsInfinity(value))
            {
                return true;
            }

            value = default;
            return false;
        }
    }
}
