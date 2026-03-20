using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using UnitsNet;

namespace MeteoSwissApi.Models.Converters
{
    internal abstract class JsonCollectionConverter<TCollection, TElement> : JsonConverter<TCollection>
        where TCollection : class
    {
        protected abstract JsonConverter<TElement> ElementConverter { get; }

        protected abstract TCollection CreateCollection(IReadOnlyCollection<TElement> values);

        public override TCollection Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
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
                    return this.CreateCollection(values);
                }

                values.Add(this.ElementConverter.Read(ref reader, typeof(TElement), options));
            }

            throw new JsonException("Unexpected end of JSON while reading collection.");
        }

        public override void Write(Utf8JsonWriter writer, TCollection value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNullValue();
                return;
            }

            writer.WriteStartArray();
            foreach (var item in (IEnumerable<TElement>)value)
            {
                this.ElementConverter.Write(writer, item, options);
            }

            writer.WriteEndArray();
        }
    }

    internal sealed class TemperatureCollectionJsonConverter : JsonCollectionConverter<IReadOnlyCollection<Temperature>, Temperature>
    {
        private static readonly TemperatureJsonConverter Converter = new TemperatureJsonConverter();
        protected override JsonConverter<Temperature> ElementConverter => Converter;
        protected override IReadOnlyCollection<Temperature> CreateCollection(IReadOnlyCollection<Temperature> values) => new List<Temperature>(values);
    }

    internal sealed class MillimeterLengthCollectionJsonConverter : JsonCollectionConverter<IReadOnlyCollection<Length>, Length>
    {
        private static readonly MillimeterLengthJsonConverter Converter = new MillimeterLengthJsonConverter();
        protected override JsonConverter<Length> ElementConverter => Converter;
        protected override IReadOnlyCollection<Length> CreateCollection(IReadOnlyCollection<Length> values) => new List<Length>(values);
    }

    internal sealed class WindDirectionCollectionJsonConverter : JsonCollectionConverter<IReadOnlyCollection<Angle>, Angle>
    {
        private static readonly WindDirectionJsonConverter Converter = new WindDirectionJsonConverter();
        protected override JsonConverter<Angle> ElementConverter => Converter;
        protected override IReadOnlyCollection<Angle> CreateCollection(IReadOnlyCollection<Angle> values) => new List<Angle>(values);
    }

    internal sealed class WindSpeedCollectionJsonConverter : JsonCollectionConverter<IReadOnlyCollection<Speed>, Speed>
    {
        private static readonly WindSpeedJsonConverter Converter = new WindSpeedJsonConverter();
        protected override JsonConverter<Speed> ElementConverter => Converter;
        protected override IReadOnlyCollection<Speed> CreateCollection(IReadOnlyCollection<Speed> values) => new List<Speed>(values);
    }

    internal sealed class EpochDateTimeCollectionJsonConverter : JsonCollectionConverter<IReadOnlyCollection<DateTime>, DateTime>
    {
        private static readonly EpochDateTimeConverter Converter = new EpochDateTimeConverter();
        protected override JsonConverter<DateTime> ElementConverter => Converter;
        protected override IReadOnlyCollection<DateTime> CreateCollection(IReadOnlyCollection<DateTime> values) => new List<DateTime>(values);
    }

    internal sealed class MinuteDurationCollectionJsonConverter : JsonCollectionConverter<IReadOnlyCollection<Duration>, Duration>
    {
        private static readonly MinuteDurationJsonConverter Converter = new MinuteDurationJsonConverter();
        protected override JsonConverter<Duration> ElementConverter => Converter;
        protected override IReadOnlyCollection<Duration> CreateCollection(IReadOnlyCollection<Duration> values) => new List<Duration>(values);
    }

    internal sealed class PercentRatioCollectionJsonConverter : JsonCollectionConverter<IReadOnlyCollection<Ratio>, Ratio>
    {
        private static readonly PercentRatioJsonConverter Converter = new PercentRatioJsonConverter();
        protected override JsonConverter<Ratio> ElementConverter => Converter;
        protected override IReadOnlyCollection<Ratio> CreateCollection(IReadOnlyCollection<Ratio> values) => new List<Ratio>(values);
    }

    internal sealed class TemperatureArrayJsonConverter : JsonCollectionConverter<Temperature[], Temperature>
    {
        private static readonly TemperatureJsonConverter Converter = new TemperatureJsonConverter();
        protected override JsonConverter<Temperature> ElementConverter => Converter;
        protected override Temperature[] CreateCollection(IReadOnlyCollection<Temperature> values) => new List<Temperature>(values).ToArray();
    }

    internal sealed class HourDurationArrayJsonConverter : JsonCollectionConverter<Duration[], Duration>
    {
        private static readonly HourDurationJsonConverter Converter = new HourDurationJsonConverter();
        protected override JsonConverter<Duration> ElementConverter => Converter;
        protected override Duration[] CreateCollection(IReadOnlyCollection<Duration> values) => new List<Duration>(values).ToArray();
    }

    internal sealed class DecimalFractionRatioArrayJsonConverter : JsonCollectionConverter<Ratio[], Ratio>
    {
        private static readonly DecimalFractionRatioJsonConverter Converter = new DecimalFractionRatioJsonConverter();
        protected override JsonConverter<Ratio> ElementConverter => Converter;
        protected override Ratio[] CreateCollection(IReadOnlyCollection<Ratio> values) => new List<Ratio>(values).ToArray();
    }

    internal sealed class MillimeterLengthArrayJsonConverter : JsonCollectionConverter<Length[], Length>
    {
        private static readonly MillimeterLengthJsonConverter Converter = new MillimeterLengthJsonConverter();
        protected override JsonConverter<Length> ElementConverter => Converter;
        protected override Length[] CreateCollection(IReadOnlyCollection<Length> values) => new List<Length>(values).ToArray();
    }
}
