using System;
using Newtonsoft.Json;
using UnitsNet;
using UnitsNet.Units;

namespace MeteoSwissApi.Models.Converters
{
    internal abstract class DurationJsonConverter : JsonConverter<Duration>
    {
        private readonly DurationUnit durationUnit;

        protected DurationJsonConverter(DurationUnit durationUnit)
        {
            this.durationUnit = durationUnit;
        }

        public override void WriteJson(JsonWriter writer, Duration value, JsonSerializer serializer)
        {
            writer.WriteValue($"{value.Value}");
        }

        public override Duration ReadJson(JsonReader reader, Type objectType, Duration existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.Value is long integer)
            {
                return Duration.From(integer, this.durationUnit);
            }

            if (reader.Value is double number)
            {
                return Duration.From(number, this.durationUnit);
            }

            return reader.Value is string stringValue && double.TryParse(stringValue, out var value)
                ? Duration.From(value, this.durationUnit)
                : default;
        }
    }
}
