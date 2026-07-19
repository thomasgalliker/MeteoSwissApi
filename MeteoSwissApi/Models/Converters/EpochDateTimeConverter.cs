using System.Text.Json;
using System.Text.Json.Serialization;

namespace MeteoSwissApi.Models.Converters
{
    /// <summary>
    /// Converts integer/long dates starting from 1970-01-01 (Epoch) to DateTime.
    /// Helpful source: https://www.epochconverter.com
    /// </summary>
    public class EpochDateTimeConverter : JsonConverter<DateTime>
    {
        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static DateTime Convert(long ms)
        {
            return Epoch.AddMilliseconds(ms);
        }

        public static long Convert(DateTime dateTime)
        {
            var utcDateTime = dateTime.ToUniversalTime();
            return (long)(utcDateTime - Epoch).TotalMilliseconds;
        }

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return default;
            }

            return Convert(reader.GetInt64());
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(Convert(value));
        }
    }
}
