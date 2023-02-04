using Newtonsoft.Json;

namespace MeteoSwissApi.Models
{
    public class WarningsOverview
    {
        [JsonProperty("warnType")]
        public WarnType WarnType { get; set; }

        [JsonProperty("warnLevel")]
        public int WarnLevel { get; set; }

        public override string ToString()
        {
            return $"WarningsOverview: WarnType={this.WarnType}, WarnLevel={this.WarnLevel}";
        }
    }
}