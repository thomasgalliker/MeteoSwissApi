using System;
using System.Collections.Generic;
using MeteoSwissApi.Models.Converters;
using Newtonsoft.Json;

namespace MeteoSwissApi.Models
{
    public class Graph
    {
        public Graph()
        {
            this.Precipitation10m = new List<double>();
            this.PrecipitationMin10m = new List<double>();
            this.PrecipitationMax10m = new List<double>();
            this.WeatherIcon3h = new List<int>();
            this.WeatherIcon3hV2 = new List<int>();
            this.WindDirection3h = new List<int>();
            this.WindSpeed3h = new List<double>();
            this.Sunrise = new List<DateTime>();
            this.Sunset = new List<DateTime>();
            this.TemperatureMin1h = new List<Temperature>();
            this.TemperatureMax1h = new List<Temperature>();
            this.TemperatureMean1h = new List<Temperature>();
            this.Precipitation1h = new List<double>();
            this.PrecipitationMin1h = new List<double>();
            this.PrecipitationMax1h = new List<double>();
        }

        [JsonProperty("start")]
        [JsonConverter(typeof(EpochDateTimeConverter))]
        public DateTime Start { get; set; }

        [JsonProperty("startLowResolution")]
        [JsonConverter(typeof(EpochDateTimeConverter))]
        public DateTime StartLowResolution { get; set; }

        [JsonProperty("precipitation10m")]
        public IReadOnlyCollection<double> Precipitation10m { get; set; }

        [JsonProperty("precipitationMin10m")]
        public IReadOnlyCollection<double> PrecipitationMin10m { get; set; }

        [JsonProperty("precipitationMax10m")]
        public IReadOnlyCollection<double> PrecipitationMax10m { get; set; }

        [JsonProperty("weatherIcon3h")]
        public IReadOnlyCollection<int> WeatherIcon3h { get; set; }

        [JsonProperty("weatherIcon3hV2")]
        public IReadOnlyCollection<int> WeatherIcon3hV2 { get; set; }

        [JsonProperty("windDirection3h")]
        public IReadOnlyCollection<int> WindDirection3h { get; set; }

        [JsonProperty("windSpeed3h")]
        public IReadOnlyCollection<double> WindSpeed3h { get; set; }

        [JsonProperty("sunrise", ItemConverterType = typeof(EpochDateTimeConverter))]
        public IReadOnlyCollection<DateTime> Sunrise { get; set; }

        [JsonProperty("sunset", ItemConverterType = typeof(EpochDateTimeConverter))]
        public IReadOnlyCollection<DateTime> Sunset { get; set; }

        [JsonProperty("temperatureMin1h", ItemConverterType = typeof(TemperatureJsonConverter))]
        public IReadOnlyCollection<Temperature> TemperatureMin1h { get; set; }

        [JsonProperty("temperatureMax1h", ItemConverterType = typeof(TemperatureJsonConverter))]
        public IReadOnlyCollection<Temperature> TemperatureMax1h { get; set; }

        [JsonProperty("temperatureMean1h", ItemConverterType = typeof(TemperatureJsonConverter))]
        public IReadOnlyCollection<Temperature> TemperatureMean1h { get; set; }

        [JsonProperty("precipitation1h")]
        public IReadOnlyCollection<double> Precipitation1h { get; set; }

        [JsonProperty("precipitationMin1h")]
        public IReadOnlyCollection<double> PrecipitationMin1h { get; set; }

        [JsonProperty("precipitationMax1h")]
        public IReadOnlyCollection<double> PrecipitationMax1h { get; set; }
    }
}