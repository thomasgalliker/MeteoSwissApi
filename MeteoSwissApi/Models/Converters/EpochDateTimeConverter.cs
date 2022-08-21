using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MeteoSwissApi.Models.Converters
{
    /// <summary>
    /// Converts integer/long dates starting from 1970-01-01 (Epoch) to DateTime.
    /// Helpful source: https://www.epochconverter.com
    /// </summary>
    public class EpochDateTimeConverter : DateTimeConverterBase
    {
        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static DateTime Convert(long ms) => Epoch.AddMilliseconds(ms);

        public static long Convert(DateTime utcDateTime) => (long)(utcDateTime - Epoch).TotalMilliseconds;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var utcDateTime = (DateTime)value;
            var ms = Convert(utcDateTime);
            writer.WriteValue(ms);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null)
            {
                return null;
            }

            var ms = System.Convert.ToInt64(reader.Value);
            var dateTime = Convert(ms);
            return dateTime;
        }
    }
}