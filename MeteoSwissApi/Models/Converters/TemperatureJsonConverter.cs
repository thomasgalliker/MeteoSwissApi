using System;
using Newtonsoft.Json;

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
                return Temperature.FromCelsius(celsiusLong);
            }

            if (reader.Value is double celsiusDouble)
            {
                return Temperature.FromCelsius(celsiusDouble);
            }

            return reader.Value is string stringValue && double.TryParse(stringValue, out var celsius)
                ? Temperature.FromCelsius(celsius)
                : default;
        }
    }
}
