using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MeteoSwissApi.Models.Converters
{
    internal class WarnTypeJsonConverter : JsonConverter<WarnType>
    {
        public override WarnType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
            {
                return (WarnType)reader.GetInt32();
            }

            throw new NotSupportedException($"Cannot convert from {reader.TokenType} to {nameof(WarnType)}");
        }

        public override void Write(Utf8JsonWriter writer, WarnType value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value.Value);
        }
    }
}
