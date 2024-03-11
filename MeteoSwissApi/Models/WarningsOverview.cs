using MeteoSwissApi.Models.Converters;
using Newtonsoft.Json;

namespace MeteoSwissApi.Models
{
    public class WarningsOverview
    {
        [JsonProperty("warnType")]
        [JsonConverter(typeof(WarnTypeJsonConverter))]
        public WarnType WarnType { get; set; }

        [JsonProperty("warnLevel")]
        [JsonConverter(typeof(WarnLevelJsonConverter))]
        public WarnLevel WarnLevel { get; set; }

        public override string ToString()
        {
            return $"WarnType={this.WarnType}, WarnLevel={this.WarnType.ToString(this.WarnLevel)}";
        }
    }
}