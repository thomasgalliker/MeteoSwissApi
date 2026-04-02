using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MeteoSwissApi.Models.Converters
{
    internal class DateTimeStringJsonConverter : JsonConverter<DateTime>
    {
        private const string DateFormat = "yyyy-MM-dd";

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                return DateTime.ParseExact(reader.GetString(), DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal);
            }

            return default;
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(DateFormat, CultureInfo.InvariantCulture));
        }
    }
}
