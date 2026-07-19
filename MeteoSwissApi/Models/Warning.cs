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

        [JsonProperty("validFrom")]
        [JsonConverter(typeof(EpochDateTimeConverter))]
        public DateTime ValidFrom { get; set; }

        [JsonProperty("validTo")]
        [JsonConverter(typeof(EpochDateTimeConverter))]
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
        public Link[] Links { get; set; }

        public override string ToString()
        {
            return
                $"WarnType={this.WarnType}, WarnLevel={this.WarnType.ToString(this.WarnLevel)}, " +
                $"Validity={this.ValidFrom}{(this.ValidTo is DateTime validTo ? $"-{validTo}" : "")} {(this.Text is string text ? $" ({text})" : "")}";
        }
    }
}
