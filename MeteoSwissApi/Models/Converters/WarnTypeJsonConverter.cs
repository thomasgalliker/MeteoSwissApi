using System.Text.Json;
using System.Text.Json.Serialization;

namespace MeteoSwissApi.Models.Converters
{
    internal sealed class WarnTypeJsonConverter : JsonConverter<WarnType>
    {
        public override WarnType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return default;
            }

            // Values MeteoSwiss adds later are kept instead of failing the whole response.
            if (reader.TokenType == JsonTokenType.Number && reader.TryGetInt32(out var value))
            {
                return WarnType.FromValueOrUnknown(value);
            }

            throw new JsonException($"Cannot convert {reader.TokenType} to {nameof(WarnType)}.");
        }

        public override void Write(Utf8JsonWriter writer, WarnType value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value.Value);
        }
    }
}
