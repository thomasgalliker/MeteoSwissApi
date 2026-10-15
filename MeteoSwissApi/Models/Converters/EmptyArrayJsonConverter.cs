using System.Text.Json;
using System.Text.Json.Serialization;

namespace MeteoSwissApi.Models.Converters
{
    /// <summary>
    /// Reads a JSON array whose elements are deserialized with the regular options,
    /// and returns an empty array instead of <c>null</c> when the JSON value is <c>null</c>.
    /// A <c>null</c> element of a reference type is skipped; a <c>null</c> element of a value type is an error.
    /// </summary>
    internal sealed class EmptyArrayJsonConverter<TElement> : JsonConverter<TElement[]>
    {
        public override bool HandleNull => true;

        public override TElement[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return Array.Empty<TElement>();
            }

            if (reader.TokenType != JsonTokenType.StartArray)
            {
                throw new JsonException($"Expected {JsonTokenType.StartArray}, got {reader.TokenType}.");
            }

            var elements = new List<TElement>();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                {
                    return elements.ToArray();
                }

                if (reader.TokenType == JsonTokenType.Null && default(TElement) is null)
                {
                    continue;
                }

                elements.Add(JsonSerializer.Deserialize<TElement>(ref reader, options)!);
            }

            throw new JsonException("Unexpected end of JSON while reading an array.");
        }

        public override void Write(Utf8JsonWriter writer, TElement[]? value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNullValue();
                return;
            }

            writer.WriteStartArray();
            foreach (var element in value)
            {
                JsonSerializer.Serialize(writer, element, options);
            }

            writer.WriteEndArray();
        }
    }
}
