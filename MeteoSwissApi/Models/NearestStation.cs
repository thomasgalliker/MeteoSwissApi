using MeteoSwissApi.Models.Converters;
using Newtonsoft.Json;
using UnitsNet;

namespace MeteoSwissApi.Models
{
    public class NearestStation : SlfStation
    {
        [JsonProperty("distance")]
        [JsonConverter(typeof(KilometerLengthJsonConverter))]
        public Length Distance { get; set; }

        [JsonProperty("bearing")]
        [JsonConverter(typeof(DegreeAngleJsonConverter))]
        public Angle Bearing { get; set; }
    }
}
