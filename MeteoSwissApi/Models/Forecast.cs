using MeteoSwissApi.Models.Converters;
using Newtonsoft.Json;

namespace MeteoSwissApi.Models
{
    public class Forecast
    {
        [JsonProperty("dayDate")]
        public string DayDate { get; set; }

        [JsonProperty("iconDay")]
        public int IconDay { get; set; }

        [JsonProperty("iconDayV2")]
        public int IconDayV2 { get; set; }

        [JsonProperty("temperatureMax")]
        [JsonConverter(typeof(TemperatureJsonConverter))]
        public Temperature TemperatureMax { get; set; }

        [JsonProperty("temperatureMin")]
        [JsonConverter(typeof(TemperatureJsonConverter))]
        public Temperature TemperatureMin { get; set; }

        [JsonProperty("precipitation")]
        public double Precipitation { get; set; }
    }
}