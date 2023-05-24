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
            if (reader.Value is long celsiusLong)
            {
                return Temperature.FromDegreesCelsius(celsiusLong);
            }

            if (reader.Value is double celsiusDouble)
            {
                return Temperature.FromDegreesCelsius(celsiusDouble);
            }

            return reader.Value is string stringValue && double.TryParse(stringValue, out var celsius)
                ? Temperature.FromDegreesCelsius(celsius)
                : default;
        }
    }
}
