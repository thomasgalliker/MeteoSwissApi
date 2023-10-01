using System;
using Newtonsoft.Json;
using UnitsNet;
using UnitsNet.Units;

namespace MeteoSwissApi.Models.Converters
{
    internal abstract class LengthJsonConverter : JsonConverter<Length>
    {
        private readonly LengthUnit lengthUnit;

        protected LengthJsonConverter(LengthUnit lengthUnit)
        {
            this.lengthUnit = lengthUnit;
        }

        public override void WriteJson(JsonWriter writer, Length value, JsonSerializer serializer)
        {
            writer.WriteValue($"{value.Value}");
        }

        public override Length ReadJson(JsonReader reader, Type objectType, Length existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.Value is long integer)
            {
                return Length.From(integer, this.lengthUnit);
            }

            if (reader.Value is double number)
            {
                return Length.From(number, this.lengthUnit);
            }

            return reader.Value is string stringValue && double.TryParse(stringValue, out var value)
                ? Length.From(value, this.lengthUnit)
                : default;
        }
    }
}
