using System.Text.Json;
using System.Text.Json.Serialization;

namespace MeteoSwissApi.Models.Converters
{
    /// <summary>
    /// Reads and writes a UTC <see cref="DateTime"/> as milliseconds since 1970-01-01T00:00:00Z (Unix epoch),
    /// the format of the timestamps in the MeteoSwiss API. A JSON <c>null</c> becomes <see cref="DateTime.MinValue"/>,
    /// and <see cref="DateTime.MinValue"/> is written as <c>null</c>. A <see cref="DateTime"/> whose kind is
    /// <see cref="DateTimeKind.Unspecified"/> is written as UTC, the convention of every timestamp in this library.
    /// </summary>
    internal sealed class EpochDateTimeJsonConverter : JsonConverter<DateTime>
    {
        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Returns the UTC date and time <paramref name="milliseconds"/> after the Unix epoch.
        /// </summary>
        internal static DateTime ToDateTime(long milliseconds)
        {
            return Epoch.AddMilliseconds(milliseconds);
        }

        /// <summary>
        /// Returns the milliseconds between the Unix epoch and <paramref name="dateTime"/>.
        /// </summary>
        internal static long ToUnixTimeMilliseconds(DateTime dateTime)
        {
            var utcDateTime = dateTime.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(dateTime, DateTimeKind.Utc) : dateTime.ToUniversalTime();
            return (long)(utcDateTime - Epoch).TotalMilliseconds;
        }

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return default;
            }

            if (reader.TokenType == JsonTokenType.Number && reader.TryGetInt64(out var milliseconds))
            {
                return ToDateTime(milliseconds);
            }

            throw new JsonException($"Cannot convert {reader.TokenType} to milliseconds since the Unix epoch.");
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            if (value == DateTime.MinValue)
            {
                writer.WriteNullValue();
                return;
            }

            writer.WriteNumberValue(ToUnixTimeMilliseconds(value));
        }
    }
}
