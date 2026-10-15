using System.Text.Json;
using System.Text.Json.Serialization;

namespace MeteoSwissApi.Models.Converters
{
    /// <summary>
    /// Reads and writes a calendar date such as <c>"2026-07-19"</c>. The date has no time and no time zone,
    /// so its <see cref="DateTime.Kind"/> is <see cref="DateTimeKind.Unspecified"/> and midnight is not converted
    /// to any time zone. A JSON <c>null</c> becomes <see cref="DateTime.MinValue"/>; any other token that is no date fails.
    /// </summary>
    internal sealed class DateTimeStringJsonConverter : JsonConverter<DateTime>
    {
        private const string DateFormat = "yyyy-MM-dd";

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return default;
            }

            if (reader.TokenType == JsonTokenType.String &&
                DateTime.TryParseExact(reader.GetString(), DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            {
                return date;
            }

            throw new JsonException($"Cannot convert {reader.TokenType} to a calendar date of the format {DateFormat}.");
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(DateFormat, CultureInfo.InvariantCulture));
        }
    }
}
