using System;
using Newtonsoft.Json;
using UnitsNet;
using UnitsNet.Units;

namespace MeteoSwissApi.Models.Converters
{
    internal abstract class RatioJsonConverter : JsonConverter<Ratio>
    {
        private readonly RatioUnit ratioUnit;

        protected RatioJsonConverter(RatioUnit ratioUnit)
        {
            this.ratioUnit = ratioUnit;
        }

        public override void WriteJson(JsonWriter writer, Ratio value, JsonSerializer serializer)
        {
            writer.WriteValue($"{value.Value}");
        }

        public override Ratio ReadJson(JsonReader reader, Type objectType, Ratio existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.Value is long integer)
            {
                return Ratio.From(integer, this.ratioUnit);
            }

            if (reader.Value is double number)
            {
                return Ratio.From(number, this.ratioUnit);
            }

            return reader.Value is string stringValue && double.TryParse(stringValue, out var value)
                ? Ratio.From(value, this.ratioUnit)
                : default;
        }
    }
}
