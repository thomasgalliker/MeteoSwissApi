using System;
using System.ComponentModel;
using MeteoSwissApi.Models.Converters;
using Newtonsoft.Json;
using UnitsNet;
using WeatherDisplay.Model.Wiewarm.Converters;

namespace MeteoSwissApi.Models
{
    public class Forecast
    {
        private int iconDayV2;

        [JsonProperty("dayDate")]
        [JsonConverter(typeof(DateTimeStringJsonConverter))]
        public DateTime DayDate { get; set; }

        [Obsolete("Use IconDayV2")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [JsonProperty("iconDay")]
        public int IconDay { get; set; }

        [JsonProperty("iconDayV2")]
        public int IconDayV2
        {
            get => this.iconDayV2;
            set
            {
                this.iconDayV2 = value;
                if (WeatherConditionCode.TryGetFromValue(value, out var weatherConditionCode))
                {
                    this.WeatherCondition = weatherConditionCode;
                }
            }
        }

        [JsonIgnore]
        public WeatherConditionCode WeatherCondition { get; private set; }

        [JsonProperty("temperatureMax")]
        [JsonConverter(typeof(TemperatureJsonConverter))]
        public Temperature TemperatureMax { get; set; }

        [JsonProperty("temperatureMin")]
        [JsonConverter(typeof(TemperatureJsonConverter))]
        public Temperature TemperatureMin { get; set; }

        [JsonProperty("precipitation")]
        [JsonConverter(typeof(MillimeterLengthJsonConverter))]
        public Length Precipitation { get; set; }
        
        [JsonProperty("precipitationMin")]
        [JsonConverter(typeof(MillimeterLengthJsonConverter))]
        public Length PrecipitationMin { get; set; }
        
        [JsonProperty("precipitationMax")]
        [JsonConverter(typeof(MillimeterLengthJsonConverter))]
        public Length PrecipitationMax { get; set; }

        public override string ToString()
        {
            return $"{this.DayDate:d} ({this.TemperatureMin.Value:N0}-{this.TemperatureMax})";
        }
    }
}