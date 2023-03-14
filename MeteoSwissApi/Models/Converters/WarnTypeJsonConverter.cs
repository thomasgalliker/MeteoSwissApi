using System;
using Newtonsoft.Json;

namespace MeteoSwissApi.Models.Converters
{
    internal class WarnTypeJsonConverter : JsonConverter<WarnType>
    {
        public override void WriteJson(JsonWriter writer, WarnType value, JsonSerializer serializer)
        {
            writer.WriteValue(value.Value);
        }

        public override WarnType ReadJson(JsonReader reader, Type objectType, WarnType existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.Value is long value)
            {
                return (WarnType)value;
            }

            throw new NotSupportedException($"Cannot convert from {reader.Value} to {nameof(WarnType)}");
        }
    }
}
