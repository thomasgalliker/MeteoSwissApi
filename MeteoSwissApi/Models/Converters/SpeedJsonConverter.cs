using System.Text.Json;
using System.Text.Json.Serialization;

namespace MeteoSwissApi.Models.Converters
{
    internal class SpeedJsonConverter : JsonConverter<Speed>
    {
        public override Speed Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
            {
                return Speed.FromKilometersPerHour(reader.GetDouble());
            }

            if (reader.TokenType == JsonTokenType.String
                && double.TryParse(reader.GetString(), NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var value))
            {
                return Speed.FromKilometersPerHour(value);
            }

            return default;
        }

        public override void Write(Utf8JsonWriter writer, Speed value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Value.ToString(CultureInfo.InvariantCulture));
        }
    }
}
