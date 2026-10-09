using System.Text.Json;
using System.Text.Json.Serialization;

namespace MeteoSwissApi.Models.Converters
{
    /// <summary>
    /// Converts integer/long dates starting from 1970-01-01 (Epoch) to DateTime.
    /// Helpful source: https://www.epochconverter.com
    /// </summary>
    [Obsolete("EpochDateTimeConverter is an implementation detail of MeteoSwissApi and will become internal in a future version.")]
    public class EpochDateTimeConverter : JsonConverter<DateTime>
    {
        private static readonly EpochDateTimeJsonConverter Converter = new EpochDateTimeJsonConverter();

        public static DateTime Convert(long ms)
        {
            return EpochDateTimeJsonConverter.ToDateTime(ms);
        }

        public static long Convert(DateTime dateTime)
        {
            return EpochDateTimeJsonConverter.ToUnixTimeMilliseconds(dateTime);
        }

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return Converter.Read(ref reader, typeToConvert, options);
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            Converter.Write(writer, value, options);
        }
    }
}
