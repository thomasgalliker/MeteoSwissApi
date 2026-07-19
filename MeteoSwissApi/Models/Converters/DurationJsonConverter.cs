using System.Text.Json;
using System.Text.Json.Serialization;

namespace MeteoSwissApi.Models.Converters
{
    internal abstract class DurationJsonConverter : JsonConverter<Duration>
    {
        private readonly DurationUnit durationUnit;

        protected DurationJsonConverter(DurationUnit durationUnit)
        {
            this.durationUnit = durationUnit;
        }

        public override Duration Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
            {
                return Duration.From(reader.GetDouble(), this.durationUnit);
            }

            if (reader.TokenType == JsonTokenType.String
                && double.TryParse(reader.GetString(), NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var value))
            {
                return Duration.From(value, this.durationUnit);
            }

            return default;
        }

        public override void Write(Utf8JsonWriter writer, Duration value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Value.ToString(CultureInfo.InvariantCulture));
        }
    }
}
