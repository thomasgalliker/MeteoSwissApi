using System.Text.Json;
using System.Text.Json.Serialization;

namespace MeteoSwissApi.Models.Converters
{
    /// <summary>
    /// Reads a length in a fixed unit from a JSON number or an invariant-culture numeric string and maps
    /// missing values (JSON <c>null</c>, unreadable values and the MeteoSwiss "no data" value 32767) to <c>null</c>.
    /// </summary>
    internal abstract class NullableLengthJsonConverter : JsonConverter<Length?>
    {
        /// <summary>
        /// MeteoSwiss encodes missing/unavailable values as 32767 (0x7FFF) in the JSON API.
        /// </summary>
        private const double NoDataValue = 32767d;

        private readonly LengthUnit lengthUnit;

        protected NullableLengthJsonConverter(LengthUnit lengthUnit)
        {
            this.lengthUnit = lengthUnit;
        }

        public override Length? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (JsonNumberReader.TryReadDouble(ref reader, out var value) && Math.Abs(value) < NoDataValue)
            {
                return Length.From(value, this.lengthUnit);
            }

            return null;
        }

        public override void Write(Utf8JsonWriter writer, Length? value, JsonSerializerOptions options)
        {
            if (value is Length length)
            {
                writer.WriteNumberValue(length.As(this.lengthUnit));
            }
            else
            {
                writer.WriteNullValue();
            }
        }
    }
}
