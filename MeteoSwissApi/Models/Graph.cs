using System;
using System.Collections.Generic;
using MeteoSwissApi.Models.Converters;
using Newtonsoft.Json;
using UnitsNet;

namespace MeteoSwissApi.Models
{
    public class GraphCompact
    {
        public GraphCompact()
        {
            this.Precipitation10m = new List<double>();
            this.PrecipitationMin10m = new List<double>();
            this.PrecipitationMax10m = new List<double>();
            this.TemperatureMin1h = new List<Temperature>();
            this.TemperatureMax1h = new List<Temperature>();
            this.TemperatureMean1h = new List<Temperature>();
            this.PrecipitationMin1h = new List<double>();
            this.PrecipitationMax1h = new List<double>();
            this.PrecipitationMean1h = new List<double>();
        }

        [JsonProperty("start")]
        [JsonConverter(typeof(EpochDateTimeConverter))]
        public DateTime Start { get; set; }

        [JsonProperty("temperatureMin1h", ItemConverterType = typeof(TemperatureJsonConverter))]
        public IReadOnlyCollection<Temperature> TemperatureMin1h { get; set; }

        [JsonProperty("temperatureMax1h", ItemConverterType = typeof(TemperatureJsonConverter))]
        public IReadOnlyCollection<Temperature> TemperatureMax1h { get; set; }

        [JsonProperty("temperatureMean1h", ItemConverterType = typeof(TemperatureJsonConverter))]
        public IReadOnlyCollection<Temperature> TemperatureMean1h { get; set; }

        [JsonProperty("precipitation10m")]
        public IReadOnlyCollection<double> Precipitation10m { get; set; }

        [JsonProperty("precipitationMin10m")]
        public IReadOnlyCollection<double> PrecipitationMin10m { get; set; }

        [JsonProperty("precipitationMax10m")]
        public IReadOnlyCollection<double> PrecipitationMax10m { get; set; }

        [JsonProperty("precipitationMin1h")]
        public IReadOnlyCollection<double> PrecipitationMin1h { get; set; }

        [JsonProperty("precipitationMax1h")]
        public IReadOnlyCollection<double> PrecipitationMax1h { get; set; }

        [JsonProperty("precipitationMean1h")]
        public IReadOnlyCollection<double> PrecipitationMean1h { get; set; }

        public override string ToString()
        {
            return $"{this.Start}";
        }
    }
}