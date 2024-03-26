using Newtonsoft.Json;

namespace MeteoSwissApi.Models
{
    public class Link
    {
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("altUrl")]
        public string AlternativeUrl { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        public override string ToString()
        {
            return $"{this.Text}";
        }
    }
}