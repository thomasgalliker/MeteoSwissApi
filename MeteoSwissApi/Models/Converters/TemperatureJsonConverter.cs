using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using UnitsNet;

namespace MeteoSwissApi.Models.Converters
{
    internal class TemperatureJsonConverter : JsonConverter<Temperature>
    {
        public override Temperature Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
            {
                return Temperature.FromDegreesCelsius(reader.GetDouble());
            }

            if (reader.TokenType == JsonTokenType.String
                && double.TryParse(reader.GetString(), NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var value))
            {
                return Temperature.FromDegreesCelsius(value);
            }

            return default;
        }

        public override void Write(Utf8JsonWriter writer, Temperature value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Value.ToString(CultureInfo.InvariantCulture));
        }
    }
}
