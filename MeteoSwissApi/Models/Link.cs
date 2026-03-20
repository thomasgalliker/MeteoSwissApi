
namespace MeteoSwissApi.Models
{
    public class Link
    {
        [JsonProperty("url")]
        public string Url { get; set; } = null!;

        [JsonProperty("altUrl")]
        public string AlternativeUrl { get; set; } = null!;

        [JsonProperty("text")]
        public string Text { get; set; } = null!;

        public override string ToString()
        {
            return $"{this.Text}";
        }
    }
}
