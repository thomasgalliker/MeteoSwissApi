using System.Text.Json;
using System.Text.Json.Serialization;

namespace MeteoSwissApi.Models.Converters
{
    /// <summary>
    /// Reads a weather icon identifier and maps the MeteoSwiss "no data" sentinel
    /// (<c>32767</c> = <see cref="short.MaxValue"/>) to <c>null</c> instead of returning it
    /// as a valid icon id.
    /// </summary>
    internal class NullableIconJsonConverter : JsonConverter<int?>
    {
        /// <summary>
        /// MeteoSwiss encodes missing/unavailable values as 32767 (0x7FFF) in the JSON API
        /// (e.g. a frozen <c>currentWeather</c> block reports <c>icon: 32767</c>).
        /// </summary>
        private const int NoDataValue = 32767;

        public override int? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
            {
                if (reader.TryGetInt32(out var intValue))
                {
                    return intValue == NoDataValue ? null : intValue;
                }

                // Fallback for numbers that are not exact int32 (e.g. "32767.0").
                var doubleValue = reader.GetDouble();
                return Math.Abs(doubleValue) >= NoDataValue ? null : (int?)(int)doubleValue;
            }

            return null;
        }

        public override void Write(Utf8JsonWriter writer, int? value, JsonSerializerOptions options)
        {
            if (value is int iconId)
            {
                writer.WriteNumberValue(iconId);
            }
            else
            {
                writer.WriteNullValue();
            }
        }
    }
}
