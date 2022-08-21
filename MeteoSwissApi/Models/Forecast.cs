using System;
using MeteoSwissApi.Models.Converters;
using Newtonsoft.Json;
using WeatherDisplay.Model.Wiewarm.Converters;

namespace MeteoSwissApi.Models
{
    public class Forecast
    {
        [JsonProperty("dayDate")]
        [JsonConverter(typeof(DateTimeStringJsonConverter))]
        public DateTime DayDate { get; set; }

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

        public override string ToString()
        {
            return $"{this.DayDate}";
        }
    }
}