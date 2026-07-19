using System.Text.Json;
using System.Text.Json.Serialization;

namespace MeteoSwissApi.Models.Converters
{
    internal class SlfStationTypeJsonConverter : JsonConverter<SlfStationType>
    {
        public override SlfStationType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return default;
            }

            if (reader.TokenType == JsonTokenType.String)
            {
                return new SlfStationType(reader.GetString()!);
            }

            throw new NotSupportedException($"Cannot convert from {reader.TokenType} to {nameof(SlfStationType)}");
        }

        public override void Write(Utf8JsonWriter writer, SlfStationType value, JsonSerializerOptions options)
        {
            if (value.Value == null)
            {
                writer.WriteNullValue();
                return;
            }

            writer.WriteStringValue(value.Value);
        }
    }
}
