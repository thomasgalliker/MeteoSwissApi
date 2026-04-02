using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using UnitsNet;

namespace MeteoSwissApi.Models.Converters
{
    internal class DegreeAngleJsonConverter : JsonConverter<Angle>
    {
        public override Angle Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
            {
                return Angle.FromDegrees(reader.GetDouble());
            }

            if (reader.TokenType == JsonTokenType.String
                && double.TryParse(reader.GetString(), NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var value))
            {
                return Angle.FromDegrees(value);
            }

            return default;
        }

        public override void Write(Utf8JsonWriter writer, Angle value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Value.ToString(CultureInfo.InvariantCulture));
        }
    }
}
