namespace MeteoSwissApi.Models
{
    public class GraphDetail
    {
        public GraphDetail()
        {
            this.WeatherIcon3h = Array.Empty<int>();
            this.WeatherIcon3hV2 = Array.Empty<int>();

            this.WindDirection3h = Array.Empty<Angle>();

            this.WindSpeed1h = Array.Empty<Speed>();
            this.WindSpeed1hQ10 = Array.Empty<Speed>();
            this.WindSpeed1hQ90 = Array.Empty<Speed>();
            this.WindSpeed3h = Array.Empty<Speed>();

            this.GustSpeed1h = Array.Empty<Speed>();
            this.GustSpeed1hQ10 = Array.Empty<Speed>();
            this.GustSpeed1hQ90 = Array.Empty<Speed>();

            this.Sunrise = Array.Empty<DateTime>();
            this.Sunset = Array.Empty<DateTime>();
            this.Sunshine1h = Array.Empty<Duration>();

            this.TemperatureMin1h = Array.Empty<Temperature>();
            this.TemperatureMax1h = Array.Empty<Temperature>();
            this.TemperatureMean1h = Array.Empty<Temperature>();

            this.Precipitation10m = Array.Empty<Length>();
            this.PrecipitationMin10m = Array.Empty<Length>();
            this.PrecipitationMax10m = Array.Empty<Length>();
            this.Precipitation1h = Array.Empty<Length>();
            this.PrecipitationMin1h = Array.Empty<Length>();
            this.PrecipitationMax1h = Array.Empty<Length>();
            this.PrecipitationProbability3h = Array.Empty<Ratio>();
        }

        [JsonProperty("start")]
        [JsonConverter(typeof(EpochDateTimeConverter))]
        public DateTime Start { get; set; }

        [JsonProperty("startLowResolution")]
        [JsonConverter(typeof(EpochDateTimeConverter))]
        public DateTime StartLowResolution { get; set; }

        [JsonProperty("temperatureMin1h")]
        [JsonConverter(typeof(TemperatureArrayJsonConverter))]
        public Temperature[] TemperatureMin1h { get; set; }

        [JsonProperty("temperatureMax1h")]
        [JsonConverter(typeof(TemperatureArrayJsonConverter))]
        public Temperature[] TemperatureMax1h { get; set; }

        [JsonProperty("temperatureMean1h")]
        [JsonConverter(typeof(TemperatureArrayJsonConverter))]
        public Temperature[] TemperatureMean1h { get; set; }

        [Obsolete("Use WeatherIcon3hV2")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [JsonProperty("weatherIcon3h")]
        public int[] WeatherIcon3h { get; set; }

        [JsonProperty("weatherIcon3hV2")]
        public int[] WeatherIcon3hV2 { get; set; }

        [JsonProperty("windDirection3h")]
        [JsonConverter(typeof(WindDirectionArrayJsonConverter))]
        public Angle[] WindDirection3h { get; set; }

        [JsonProperty("windSpeed1h")]
        [JsonConverter(typeof(WindSpeedArrayJsonConverter))]
        public Speed[] WindSpeed1h { get; set; }
        
        [JsonProperty("windSpeed1hq10")]
        [JsonConverter(typeof(WindSpeedArrayJsonConverter))]
        public Speed[] WindSpeed1hQ10 { get; set; }
        
        [JsonProperty("windSpeed1hq90")]
        [JsonConverter(typeof(WindSpeedArrayJsonConverter))]
        public Speed[] WindSpeed1hQ90 { get; set; }
        
        [JsonProperty("windSpeed3h")]
        [JsonConverter(typeof(WindSpeedArrayJsonConverter))]
        public Speed[] WindSpeed3h { get; set; }
        
        [JsonProperty("gustSpeed1h")]
        [JsonConverter(typeof(WindSpeedArrayJsonConverter))]
        public Speed[] GustSpeed1h { get; set; }
        
        [JsonProperty("gustSpeed1hq10")]
        [JsonConverter(typeof(WindSpeedArrayJsonConverter))]
        public Speed[] GustSpeed1hQ10 { get; set; }

        [JsonProperty("gustSpeed1hq90")]
        [JsonConverter(typeof(WindSpeedArrayJsonConverter))]
        public Speed[] GustSpeed1hQ90 { get; set; }

        [JsonProperty("sunrise")]
        [JsonConverter(typeof(EpochDateTimeArrayJsonConverter))]
        public DateTime[] Sunrise { get; set; }

        [JsonProperty("sunset")]
        [JsonConverter(typeof(EpochDateTimeArrayJsonConverter))]
        public DateTime[] Sunset { get; set; }

        [JsonProperty("sunshine1h")]
        [JsonConverter(typeof(MinuteDurationArrayJsonConverter))]
        public Duration[] Sunshine1h { get; set; }
        
        [JsonProperty("precipitation10m")]
        [JsonConverter(typeof(MillimeterLengthArrayJsonConverter))]
        public Length[] Precipitation10m { get; set; }

        [JsonProperty("precipitationMin10m")]
        [JsonConverter(typeof(MillimeterLengthArrayJsonConverter))]
        public Length[] PrecipitationMin10m { get; set; }

        [JsonProperty("precipitationMax10m")]
        [JsonConverter(typeof(MillimeterLengthArrayJsonConverter))]
        public Length[] PrecipitationMax10m { get; set; }

        [JsonProperty("precipitation1h")]
        [JsonConverter(typeof(MillimeterLengthArrayJsonConverter))]
        public Length[] Precipitation1h { get; set; }

        [JsonProperty("precipitationMin1h")]
        [JsonConverter(typeof(MillimeterLengthArrayJsonConverter))]
        public Length[] PrecipitationMin1h { get; set; }

        [JsonProperty("precipitationMax1h")]
        [JsonConverter(typeof(MillimeterLengthArrayJsonConverter))]
        public Length[] PrecipitationMax1h { get; set; }
        
        [JsonProperty("precipitationProbability3h")]
        [JsonConverter(typeof(PercentRatioArrayJsonConverter))]
        public Ratio[] PrecipitationProbability3h { get; set; }

        public override string ToString()
        {
            return $"{this.Start}";
        }
    }
}

