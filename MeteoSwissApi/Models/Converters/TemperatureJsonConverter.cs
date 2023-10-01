using System;
using Newtonsoft.Json;
using UnitsNet;

namespace MeteoSwissApi.Models.Converters
{
    internal class TemperatureJsonConverter : JsonConverter<Temperature>
    {
        public override void WriteJson(JsonWriter writer, Temperature value, JsonSerializer serializer)
        {
            writer.WriteValue($"{value.Value}");
        }

        public override Temperature ReadJson(JsonReader reader, Type objectType, Temperature existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.Value is long integer)
            {
                return Temperature.FromDegreesCelsius(integer);
            }

            if (reader.Value is double number)
            {
                return Temperature.FromDegreesCelsius(number);
            }

            return reader.Value is string stringValue && double.TryParse(stringValue, out var value)
                ? Temperature.FromDegreesCelsius(value)
                : default;
        }
    }
}
