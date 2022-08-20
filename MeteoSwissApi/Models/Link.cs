using Newtonsoft.Json;

namespace MeteoSwissApi.Models
{
    public class Link
    {
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }
    }
}