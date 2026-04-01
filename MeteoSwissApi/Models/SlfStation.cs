using MeteoSwissApi.Models.Converters;
using UnitsNet;

namespace MeteoSwissApi.Models
{
    public class SlfStation
    {
        [JsonProperty("network")]
        public string Network { get; set; } = null!;

        [JsonProperty("code")]
        public string Code { get; set; } = null!;

        [JsonProperty("type")]
        public SlfStationType Type { get; set; }

        [JsonProperty("elevation")]
        [JsonConverter(typeof(MeterLengthJsonConverter))]
        public Length Elevation { get; set; }

        [JsonProperty("label")]
        public string Name { get; set; } = null!;

        [JsonProperty("manual")]
        public bool Manual { get; set; }

        [JsonProperty("location")]
        public SlfLocation Location { get; set; } = null!;

        public override string ToString()
        {
            return $"{this.Code} ({(this.Manual ? "manual" : "automatic")})";
        }
    }
}
