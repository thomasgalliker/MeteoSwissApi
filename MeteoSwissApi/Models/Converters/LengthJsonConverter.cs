using System.Text.Json;
using System.Text.Json.Serialization;

namespace MeteoSwissApi.Models.Converters
{
    internal abstract class LengthJsonConverter : JsonConverter<Length>
    {
        private readonly LengthUnit lengthUnit;

        protected LengthJsonConverter(LengthUnit lengthUnit)
        {
            this.lengthUnit = lengthUnit;
        }

        public override Length Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
            {
                return Length.From(reader.GetDouble(), this.lengthUnit);
            }

            if (reader.TokenType == JsonTokenType.String
                && double.TryParse(reader.GetString(), NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var value))
            {
                return Length.From(value, this.lengthUnit);
            }

            return default;
        }

        public override void Write(Utf8JsonWriter writer, Length value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Value.ToString(CultureInfo.InvariantCulture));
        }
    }
}
