using System.Text.Json;
using System.Text.Json.Serialization;

namespace MeteoSwissApi.Models.Converters
{
    internal sealed class WarnLevelJsonConverter : JsonConverter<WarnLevel>
    {
        public override WarnLevel Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return WarnLevel.NoWarnLevel;
            }

            // Values MeteoSwiss adds later are kept instead of failing the whole response.
            if (reader.TokenType == JsonTokenType.Number && reader.TryGetInt32(out var value))
            {
                return WarnLevel.FromValueOrUnknown(value);
            }

            throw new JsonException($"Cannot convert {reader.TokenType} to {nameof(WarnLevel)}.");
        }

        public override void Write(Utf8JsonWriter writer, WarnLevel value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value.Level);
        }
    }
}
