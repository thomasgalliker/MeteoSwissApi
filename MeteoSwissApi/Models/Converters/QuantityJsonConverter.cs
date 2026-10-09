using System.Text.Json;
using System.Text.Json.Serialization;

namespace MeteoSwissApi.Models.Converters
{
    /// <summary>
    /// Reads a UnitsNet quantity in the unit of the converter from a JSON number or an invariant-culture numeric string,
    /// and writes it as a JSON number in that unit. A token that is no number, such as <c>null</c>, becomes the default quantity.
    /// </summary>
    internal abstract class QuantityJsonConverter<TQuantity> : JsonConverter<TQuantity> where TQuantity : struct
    {
        private readonly Func<double, TQuantity> fromValue;
        private readonly Func<TQuantity, double> toValue;

        /// <param name="fromValue">Creates the quantity from a value in the unit of the converter.</param>
        /// <param name="toValue">Returns the value of the quantity in the unit of the converter.</param>
        protected QuantityJsonConverter(Func<double, TQuantity> fromValue, Func<TQuantity, double> toValue)
        {
            this.fromValue = fromValue;
            this.toValue = toValue;
        }

        public override TQuantity Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return JsonNumberReader.TryReadDouble(ref reader, out var value)
                ? this.fromValue(value)
                : default;
        }

        public override void Write(Utf8JsonWriter writer, TQuantity value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(this.toValue(value));
        }
    }
}
