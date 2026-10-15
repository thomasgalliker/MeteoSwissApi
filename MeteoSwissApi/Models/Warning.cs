namespace MeteoSwissApi.Models
{
    public class Warning
    {
        public Warning()
        {
            this.Links = Array.Empty<Link>();
        }

        [JsonProperty("warnType")]
        [JsonConverter(typeof(WarnTypeJsonConverter))]
        public WarnType WarnType { get; set; }

        [JsonProperty("warnLevel")]
        [JsonConverter(typeof(WarnLevelJsonConverter))]
        public WarnLevel WarnLevel { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; } = null!;

        /// <summary>
        /// The start of the validity, in UTC.
        /// </summary>
        [JsonProperty("validFrom")]
        [JsonConverter(typeof(EpochDateTimeJsonConverter))]
        public DateTime ValidFrom { get; set; }

        /// <summary>
        /// The end of the validity, in UTC, or <c>null</c> if the warning has no end yet.
        /// </summary>
        [JsonProperty("validTo")]
        [JsonConverter(typeof(EpochDateTimeJsonConverter))]
        public DateTime? ValidTo { get; set; }

        [JsonProperty("ordering")]
        public string Ordering { get; set; } = null!;

        [JsonProperty("regionId")]
        public int RegionId { get; set; }

        [JsonProperty("htmlText")]
        public string HtmlText { get; set; } = null!;

        [JsonProperty("outlook")]
        public bool Outlook { get; set; }

        [JsonProperty("links")]
        [JsonConverter(typeof(EmptyArrayJsonConverter<Link>))]
        public Link[] Links { get; set; }

        public override string ToString()
        {
            return
                $"WarnType={this.WarnType}, WarnLevel={this.WarnLevel}, " +
                $"Validity={this.ValidFrom}{(this.ValidTo is DateTime validTo ? $"-{validTo}" : "")} {(this.Text is string text ? $" ({text})" : "")}";
        }
    }
}
