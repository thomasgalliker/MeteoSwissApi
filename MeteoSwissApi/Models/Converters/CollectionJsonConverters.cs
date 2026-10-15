using System.Text.Json;
using System.Text.Json.Serialization;

namespace MeteoSwissApi.Models.Converters
{
    /// <summary>
    /// Reads a JSON array into an array, converting each element with the given element converter.
    /// A JSON <c>null</c> becomes an empty array.
    /// </summary>
    internal abstract class JsonArrayConverter<TElement> : JsonConverter<TElement[]>
    {
        private readonly JsonConverter<TElement> elementConverter;

        protected JsonArrayConverter(JsonConverter<TElement> elementConverter)
        {
            this.elementConverter = elementConverter;
        }

        /// <summary>
        /// Lets <see cref="Read"/> see JSON <c>null</c>, so that a missing collection becomes empty instead of <c>null</c>.
        /// </summary>
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

            var values = new List<TElement>();

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                {
                    return values.ToArray();
                }

                values.Add(this.elementConverter.Read(ref reader, typeof(TElement), options)!);
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

            foreach (var item in value)
            {
                this.elementConverter.Write(writer, item, options);
            }

            writer.WriteEndArray();
        }
    }

    internal sealed class WindDirectionArrayJsonConverter : JsonArrayConverter<Angle>
    {
        public WindDirectionArrayJsonConverter()
            : base(new DegreeAngleJsonConverter())
        {
        }
    }

    internal sealed class WindSpeedArrayJsonConverter : JsonArrayConverter<Speed>
    {
        public WindSpeedArrayJsonConverter()
            : base(new SpeedJsonConverter())
        {
        }
    }

    internal sealed class EpochDateTimeArrayJsonConverter : JsonArrayConverter<DateTime>
    {
        public EpochDateTimeArrayJsonConverter()
            : base(new EpochDateTimeJsonConverter())
        {
        }
    }

    internal sealed class MinuteDurationArrayJsonConverter : JsonArrayConverter<Duration>
    {
        public MinuteDurationArrayJsonConverter()
            : base(new MinuteDurationJsonConverter())
        {
        }
    }

    internal sealed class HourDurationArrayJsonConverter : JsonArrayConverter<Duration>
    {
        public HourDurationArrayJsonConverter()
            : base(new HourDurationJsonConverter())
        {
        }
    }

    internal sealed class PercentRatioArrayJsonConverter : JsonArrayConverter<Ratio>
    {
        public PercentRatioArrayJsonConverter()
            : base(new PercentRatioJsonConverter())
        {
        }
    }

    internal sealed class DecimalFractionRatioArrayJsonConverter : JsonArrayConverter<Ratio>
    {
        public DecimalFractionRatioArrayJsonConverter()
            : base(new DecimalFractionRatioJsonConverter())
        {
        }
    }

    internal sealed class TemperatureArrayJsonConverter : JsonArrayConverter<Temperature>
    {
        public TemperatureArrayJsonConverter()
            : base(new TemperatureJsonConverter())
        {
        }
    }

    internal sealed class TemperatureDeltaArrayJsonConverter : JsonArrayConverter<TemperatureDelta>
    {
        public TemperatureDeltaArrayJsonConverter()
            : base(new TemperatureDeltaJsonConverter())
        {
        }
    }

    internal sealed class MillimeterLengthArrayJsonConverter : JsonArrayConverter<Length>
    {
        public MillimeterLengthArrayJsonConverter()
            : base(new MillimeterLengthJsonConverter())
        {
        }
    }
}
