namespace MeteoSwissApi.Models
{
    public class Link
    {
        [JsonProperty("url")]
        public string Url { get; set; } = null!;

        /// <summary>
        /// Alternative URL of the link, or <c>null</c> if the API sends none.
        /// The MeteoSwiss API v3 sends no alternative URLs.
        /// </summary>
        [JsonProperty("altUrl")]
        public string? AlternativeUrl { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; } = null!;

        public override string ToString()
        {
            return $"{this.Text}";
        }
    }
}
