namespace MeteoSwissApi.Models
{
    public class GraphCompact
    {
        public GraphCompact()
        {
            this.TemperatureMin1h = Array.Empty<Temperature>();
            this.TemperatureMax1h = Array.Empty<Temperature>();
            this.TemperatureMean1h = Array.Empty<Temperature>();
            this.Precipitation10m = Array.Empty<Length>();
            this.PrecipitationMin10m = Array.Empty<Length>();
            this.PrecipitationMax10m = Array.Empty<Length>();
            this.PrecipitationMin1h = Array.Empty<Length>();
            this.PrecipitationMax1h = Array.Empty<Length>();
            this.PrecipitationMean1h = Array.Empty<Length>();
        }

        [JsonProperty("start")]
        [JsonConverter(typeof(EpochDateTimeConverter))]
        public DateTime Start { get; set; }

        [JsonProperty("temperatureMin1h")]
        [JsonConverter(typeof(TemperatureArrayJsonConverter))]
        public Temperature[] TemperatureMin1h { get; set; }

        [JsonProperty("temperatureMax1h")]
        [JsonConverter(typeof(TemperatureArrayJsonConverter))]
        public Temperature[] TemperatureMax1h { get; set; }

        [JsonProperty("temperatureMean1h")]
        [JsonConverter(typeof(TemperatureArrayJsonConverter))]
        public Temperature[] TemperatureMean1h { get; set; }

        [JsonProperty("precipitation10m")]
        [JsonConverter(typeof(MillimeterLengthArrayJsonConverter))]
        public Length[] Precipitation10m { get; set; }

        [JsonProperty("precipitationMin10m")]
        [JsonConverter(typeof(MillimeterLengthArrayJsonConverter))]
        public Length[] PrecipitationMin10m { get; set; }

        [JsonProperty("precipitationMax10m")]
        [JsonConverter(typeof(MillimeterLengthArrayJsonConverter))]
        public Length[] PrecipitationMax10m { get; set; }

        [JsonProperty("precipitationMin1h")]
        [JsonConverter(typeof(MillimeterLengthArrayJsonConverter))]
        public Length[] PrecipitationMin1h { get; set; }

        [JsonProperty("precipitationMax1h")]
        [JsonConverter(typeof(MillimeterLengthArrayJsonConverter))]
        public Length[] PrecipitationMax1h { get; set; }

        [JsonProperty("precipitationMean1h")]
        [JsonConverter(typeof(MillimeterLengthArrayJsonConverter))]
        public Length[] PrecipitationMean1h { get; set; }

        public override string ToString()
        {
            return $"{this.Start}";
        }
    }
}

