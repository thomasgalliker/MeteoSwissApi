using System;
using Newtonsoft.Json;

namespace MeteoSwissApi.Models.Converters
{
    internal class WarnLevelJsonConverter : JsonConverter<WarnLevel>
    {
        public override void WriteJson(JsonWriter writer, WarnLevel value, JsonSerializer serializer)
        {
            writer.WriteValue(value.Level);
        }

        public override WarnLevel ReadJson(JsonReader reader, Type objectType, WarnLevel existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.Value is long level)
            {
                return (WarnLevel)level;
            }

            throw new NotSupportedException($"Cannot convert from {reader.Value} to {nameof(WarnLevel)}");
        }
    }
}
