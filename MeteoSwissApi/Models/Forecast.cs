namespace MeteoSwissApi.Models
{
    public class Forecast
    {
        private int? iconDayV2;

        /// <summary>
        /// The day of the forecast as calendar date: midnight, <see cref="DateTimeKind.Unspecified"/>, no time zone.
        /// </summary>
        [JsonProperty("dayDate")]
        [JsonConverter(typeof(DateTimeStringJsonConverter))]
        public DateTime DayDate { get; set; }

        [Obsolete("Use IconDayV2")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [JsonProperty("iconDay")]
        [JsonConverter(typeof(NullableIconJsonConverter))]
        public int? IconDay { get; set; }

        [JsonProperty("iconDayV2")]
        [JsonConverter(typeof(NullableIconJsonConverter))]
        public int? IconDayV2
        {
            get => this.iconDayV2;
            set
            {
                this.iconDayV2 = value;
                if (value is int iconId && WeatherConditionCode.TryGetFromValue(iconId, out var weatherConditionCode))
                {
                    this.WeatherCondition = weatherConditionCode;
                }
            }
        }

        [JsonIgnore]
        public WeatherConditionCode WeatherCondition { get; private set; }

        /// <summary>
        /// Maximum temperature of the day, or <c>null</c> if MeteoSwiss reports no value.
        /// </summary>
        [JsonProperty("temperatureMax")]
        [JsonConverter(typeof(NullableTemperatureJsonConverter))]
        public Temperature? TemperatureMax { get; set; }

        /// <summary>
        /// Minimum temperature of the day, or <c>null</c> if MeteoSwiss reports no value.
        /// </summary>
        [JsonProperty("temperatureMin")]
        [JsonConverter(typeof(NullableTemperatureJsonConverter))]
        public Temperature? TemperatureMin { get; set; }

        /// <summary>
        /// Expected precipitation of the day, or <c>null</c> if MeteoSwiss reports no value.
        /// </summary>
        [JsonProperty("precipitation")]
        [JsonConverter(typeof(NullableMillimeterLengthJsonConverter))]
        public Length? Precipitation { get; set; }

        /// <summary>
        /// Lower bound of the expected precipitation, or <c>null</c> if MeteoSwiss reports no value.
        /// </summary>
        [JsonProperty("precipitationMin")]
        [JsonConverter(typeof(NullableMillimeterLengthJsonConverter))]
        public Length? PrecipitationMin { get; set; }

        /// <summary>
        /// Upper bound of the expected precipitation, or <c>null</c> if MeteoSwiss reports no value.
        /// </summary>
        [JsonProperty("precipitationMax")]
        [JsonConverter(typeof(NullableMillimeterLengthJsonConverter))]
        public Length? PrecipitationMax { get; set; }

        public override string ToString()
        {
            return $"{this.DayDate:d} ({this.TemperatureMin} - {this.TemperatureMax})";
        }
    }
}
