using System.Text.Json;
using System.Text.Json.Serialization;

namespace MeteoSwissApi.Models.Converters
{
    /// <summary>
    /// Reads an ISO 8601 timestamp such as <c>"2026-10-03T09:30:00Z"</c> as UTC, whatever offset it carries, and writes
    /// UTC with the suffix <c>Z</c>. Without this converter, System.Text.Json turns a timestamp with an offset other
    /// than <c>Z</c> into the local time of the machine. A <see cref="DateTime"/> whose kind is
    /// <see cref="DateTimeKind.Unspecified"/> is written as UTC, the convention of every timestamp in this library.
    /// </summary>
    internal sealed class UtcDateTimeJsonConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String &&
                DateTimeOffset.TryParse(reader.GetString(), CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var timestamp))
            {
                return timestamp.UtcDateTime;
            }

            throw new JsonException($"Cannot convert {reader.TokenType} to a UTC {nameof(DateTime)}.");
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            var utcValue = value.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(value, DateTimeKind.Utc) : value.ToUniversalTime();
            writer.WriteStringValue(utcValue);
        }
    }
}
