using System.Text.Json;
using System.Text.Json.Serialization;

namespace MeteoSwissApi.Models.Converters
{
    internal class WarnLevelJsonConverter : JsonConverter<WarnLevel>
    {
        public override WarnLevel Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
            {
                return (WarnLevel)reader.GetInt32();
            }

            throw new NotSupportedException($"Cannot convert from {reader.TokenType} to {nameof(WarnLevel)}");
        }

        public override void Write(Utf8JsonWriter writer, WarnLevel value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value.Level);
        }
    }
}
