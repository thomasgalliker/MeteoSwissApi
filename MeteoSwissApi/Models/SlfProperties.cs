namespace MeteoSwissApi.Models
{
    internal sealed class SlfProperties : SlfStation
    {
        [JsonProperty("value")]
        public decimal? Value { get; set; }

        [JsonProperty("velocity")]
        public decimal? Velocity { get; set; }

        [JsonProperty("direction")]
        public decimal? Direction { get; set; }

        /// <summary>
        /// The time of the measurement, in UTC, or <c>null</c> if the station reported no value.
        /// </summary>
        [JsonProperty("timestamp")]
        [JsonConverter(typeof(UtcDateTimeJsonConverter))]
        public DateTime? Timestamp { get; set; }
    }
}
