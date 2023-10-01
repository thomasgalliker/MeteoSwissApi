using System;
using Newtonsoft.Json;
using UnitsNet;

namespace MeteoSwissApi.Models.Converters
{
    internal class WindSpeedJsonConverter : JsonConverter<Speed>
    {
        public override void WriteJson(JsonWriter writer, Speed value, JsonSerializer serializer)
        {
            writer.WriteValue($"{value.Value}");
        }

        public override Speed ReadJson(JsonReader reader, Type objectType, Speed existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.Value is long integer)
            {
                return Speed.FromKilometersPerHour(integer);
            }

            if (reader.Value is double number)
            {
                return Speed.FromKilometersPerHour(number);
            }

            return reader.Value is string stringValue && double.TryParse(stringValue, out var value)
                ? Speed.FromKilometersPerHour(value)
                : default;
        }
    }
}
