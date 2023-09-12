using System;
using System.ComponentModel;
using System.Diagnostics;
using MeteoSwissApi.Models.Converters;
using Newtonsoft.Json;
using UnitsNet;

namespace MeteoSwissApi.Models
{
    public class CurrentWeather
    {
        private int iconV2;

        [JsonProperty("time")]
        [JsonConverter(typeof(EpochDateTimeConverter))]
        public DateTime Time { get; set; }

        [Obsolete("Use IconV2")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [JsonProperty("icon")]
        public int Icon { get; set; }

        [JsonProperty("iconV2")]
        public int IconV2
        {
            get => this.iconV2;
            set
            {
                this.iconV2 = value;
                if (WeatherConditionCode.TryGetFromValue(value, out var weatherConditionCode))
                {
                    this.WeatherCondition = weatherConditionCode;
                }
            }
        }

        [JsonIgnore]
        public WeatherConditionCode WeatherCondition { get; private set; }

        [JsonProperty("temperature")]
        [JsonConverter(typeof(TemperatureJsonConverter))]
        public Temperature Temperature { get; set; }

        public override string ToString()
        {
            return $"{this.Time:g} ({this.Temperature})";
        }
    }
}