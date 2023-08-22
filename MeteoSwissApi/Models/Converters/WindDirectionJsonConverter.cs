using System;
using Newtonsoft.Json;
using UnitsNet;

namespace MeteoSwissApi.Models.Converters
{
    internal class WindDirectionJsonConverter : JsonConverter<Angle>
    {
        public override void WriteJson(JsonWriter writer, Angle value, JsonSerializer serializer)
        {
            writer.WriteValue($"{value.Value}");
        }

        public override Angle ReadJson(JsonReader reader, Type objectType, Angle existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.Value is long integer)
            {
                return Angle.FromDegrees(integer);
            }

            if (reader.Value is double number)
            {
                return Angle.FromDegrees(number);
            }

            return reader.Value is string stringValue && double.TryParse(stringValue, out var value)
                ? Angle.FromDegrees(value)
                : default;
        }
    }
}
