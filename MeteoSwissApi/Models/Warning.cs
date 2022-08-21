using System.Collections.Generic;
using Newtonsoft.Json;

namespace MeteoSwissApi.Models
{
    public class Warning
    {
        public Warning()
        {
            this.Links = new List<Link>();
        }

        [JsonProperty("warnType")]
        public int WarnType { get; set; }

        [JsonProperty("warnLevel")]
        public int WarnLevel { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("validFrom")]
        public long ValidFrom { get; set; }

        [JsonProperty("ordering")]
        public string Ordering { get; set; }

        [JsonProperty("htmlText")]
        public string HtmlText { get; set; }

        [JsonProperty("outlook")]
        public bool Outlook { get; set; }

        [JsonProperty("links")]
        public IReadOnlyCollection<Link> Links { get; set; }

        public override string ToString()
        {
            return $"{this.Text}";
        }
    }
}