namespace MeteoSwissApi.Models
{
    public class CurrentWeather
    {
        private int? iconV2;

        /// <summary>
        /// The time of the observation, in UTC.
        /// </summary>
        [JsonProperty("time")]
        [JsonConverter(typeof(EpochDateTimeJsonConverter))]
        public DateTime Time { get; set; }

        [Obsolete("Use IconV2")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [JsonProperty("icon")]
        [JsonConverter(typeof(NullableIconJsonConverter))]
        public int? Icon { get; set; }

        [JsonProperty("iconV2")]
        [JsonConverter(typeof(NullableIconJsonConverter))]
        public int? IconV2
        {
            get => this.iconV2;
            set
            {
                this.iconV2 = value;
                if (value is int iconId && WeatherConditionCode.TryGetFromValue(iconId, out var weatherConditionCode))
                {
                    this.WeatherCondition = weatherConditionCode;
                }
            }
        }

        [JsonIgnore]
        public WeatherConditionCode WeatherCondition { get; private set; }

        [JsonProperty("temperature")]
        [JsonConverter(typeof(NullableTemperatureJsonConverter))]
        public Temperature? Temperature { get; set; }

        public override string ToString()
        {
            return $"{this.Time:g} ({this.Temperature})";
        }
    }
}
