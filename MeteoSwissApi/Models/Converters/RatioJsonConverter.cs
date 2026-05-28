using System.Text.Json;
using System.Text.Json.Serialization;

namespace MeteoSwissApi.Models.Converters
{
    internal abstract class RatioJsonConverter : JsonConverter<Ratio>
    {
        private readonly RatioUnit ratioUnit;

        protected RatioJsonConverter(RatioUnit ratioUnit)
        {
            this.ratioUnit = ratioUnit;
        }

        public override Ratio Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
            {
                return Ratio.From(reader.GetDouble(), this.ratioUnit);
            }

            if (reader.TokenType == JsonTokenType.String
                && double.TryParse(reader.GetString(), NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var value))
            {
                return Ratio.From(value, this.ratioUnit);
            }

            return default;
        }

        public override void Write(Utf8JsonWriter writer, Ratio value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Value.ToString(CultureInfo.InvariantCulture));
        }
    }
}
