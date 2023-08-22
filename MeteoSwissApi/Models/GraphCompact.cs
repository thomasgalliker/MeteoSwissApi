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
            this.TemperatureMin1h = new List<Temperature>();
            this.TemperatureMax1h = new List<Temperature>();
            this.TemperatureMean1h = new List<Temperature>();
            this.Precipitation10m = new List<Length>();
            this.PrecipitationMin10m = new List<Length>();
            this.PrecipitationMax10m = new List<Length>();
            this.PrecipitationMin1h = new List<Length>();
            this.PrecipitationMax1h = new List<Length>();
            this.PrecipitationMean1h = new List<Length>();
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

        [JsonProperty("precipitation10m", ItemConverterType = typeof(PrecipitationJsonConverter))]
        public IReadOnlyCollection<Length> Precipitation10m { get; set; }

        [JsonProperty("precipitationMin10m", ItemConverterType = typeof(PrecipitationJsonConverter))]
        public IReadOnlyCollection<Length> PrecipitationMin10m { get; set; }

        [JsonProperty("precipitationMax10m", ItemConverterType = typeof(PrecipitationJsonConverter))]
        public IReadOnlyCollection<Length> PrecipitationMax10m { get; set; }

        [JsonProperty("precipitationMin1h", ItemConverterType = typeof(PrecipitationJsonConverter))]
        public IReadOnlyCollection<Length> PrecipitationMin1h { get; set; }

        [JsonProperty("precipitationMax1h", ItemConverterType = typeof(PrecipitationJsonConverter))]
        public IReadOnlyCollection<Length> PrecipitationMax1h { get; set; }

        [JsonProperty("precipitationMean1h", ItemConverterType = typeof(PrecipitationJsonConverter))]
        public IReadOnlyCollection<Length> PrecipitationMean1h { get; set; }

        public override string ToString()
        {
            return $"{this.Start}";
        }
    }
}