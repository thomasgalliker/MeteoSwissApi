using System;
using System.Collections.Generic;
using System.ComponentModel;
using MeteoSwissApi.Models.Converters;
using Newtonsoft.Json;
using UnitsNet;

namespace MeteoSwissApi.Models
{
    public class GraphDetail
    {
        public GraphDetail()
        {
            this.WeatherIcon3h = new List<int>();
            this.WeatherIcon3hV2 = new List<int>();

            this.WindDirection3h = new List<Angle>();
            this.WindSpeed1h = new List<Speed>();
            this.WindSpeed1hQ10 = new List<Speed>();
            this.WindSpeed1hQ90 = new List<Speed>();
            this.WindSpeed3h = new List<Speed>();

            this.GustSpeed1h = new List<Speed>();
            this.GustSpeed1hQ10 = new List<Speed>();
            this.GustSpeed1hQ90 = new List<Speed>();

            this.Sunrise = new List<DateTime>();
            this.Sunset = new List<DateTime>();
            this.Sunshine1h = new List<Duration>();

            this.TemperatureMin1h = new List<Temperature>();
            this.TemperatureMax1h = new List<Temperature>();
            this.TemperatureMean1h = new List<Temperature>();

            this.Precipitation10m = new List<Length>();
            this.PrecipitationMin10m = new List<Length>();
            this.PrecipitationMax10m = new List<Length>();
            this.Precipitation1h = new List<Length>();
            this.PrecipitationMin1h = new List<Length>();
            this.PrecipitationMax1h = new List<Length>();
            this.PrecipitationProbability3h = new List<Ratio>();
        }

        [JsonProperty("start")]
        [JsonConverter(typeof(EpochDateTimeConverter))]
        public DateTime Start { get; set; }

        [JsonProperty("startLowResolution")]
        [JsonConverter(typeof(EpochDateTimeConverter))]
        public DateTime StartLowResolution { get; set; }

        [JsonProperty("temperatureMin1h", ItemConverterType = typeof(TemperatureJsonConverter))]
        public IReadOnlyCollection<Temperature> TemperatureMin1h { get; set; }

        [JsonProperty("temperatureMax1h", ItemConverterType = typeof(TemperatureJsonConverter))]
        public IReadOnlyCollection<Temperature> TemperatureMax1h { get; set; }

        [JsonProperty("temperatureMean1h", ItemConverterType = typeof(TemperatureJsonConverter))]
        public IReadOnlyCollection<Temperature> TemperatureMean1h { get; set; }

        [Obsolete("Use WeatherIcon3hV2")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [JsonProperty("weatherIcon3h")]
        public IReadOnlyCollection<int> WeatherIcon3h { get; set; }

        [JsonProperty("weatherIcon3hV2")]
        public IReadOnlyCollection<int> WeatherIcon3hV2 { get; set; }

        [JsonProperty("windDirection3h", ItemConverterType = typeof(WindDirectionJsonConverter))]
        public IReadOnlyCollection<Angle> WindDirection3h { get; set; }

        [JsonProperty("windSpeed1h", ItemConverterType = typeof(WindSpeedJsonConverter))]
        public IReadOnlyCollection<Speed> WindSpeed1h { get; set; }
        
        [JsonProperty("windSpeed1hq10", ItemConverterType = typeof(WindSpeedJsonConverter))]
        public IReadOnlyCollection<Speed> WindSpeed1hQ10 { get; set; }
        
        [JsonProperty("windSpeed1hq90", ItemConverterType = typeof(WindSpeedJsonConverter))]
        public IReadOnlyCollection<Speed> WindSpeed1hQ90 { get; set; }
        
        [JsonProperty("windSpeed3h", ItemConverterType = typeof(WindSpeedJsonConverter))]
        public IReadOnlyCollection<Speed> WindSpeed3h { get; set; }
        
        [JsonProperty("gustSpeed1h", ItemConverterType = typeof(WindSpeedJsonConverter))]
        public IReadOnlyCollection<Speed> GustSpeed1h { get; set; }
        
        [JsonProperty("gustSpeed1hq10", ItemConverterType = typeof(WindSpeedJsonConverter))]
        public IReadOnlyCollection<Speed> GustSpeed1hQ10 { get; set; }

        [JsonProperty("gustSpeed1hq90", ItemConverterType = typeof(WindSpeedJsonConverter))]
        public IReadOnlyCollection<Speed> GustSpeed1hQ90 { get; set; }

        [JsonProperty("sunrise", ItemConverterType = typeof(EpochDateTimeConverter))]
        public IReadOnlyCollection<DateTime> Sunrise { get; set; }

        [JsonProperty("sunset", ItemConverterType = typeof(EpochDateTimeConverter))]
        public IReadOnlyCollection<DateTime> Sunset { get; set; }

        [JsonProperty("sunshine1h", ItemConverterType = typeof(MinuteDurationJsonConverter))]
        public IReadOnlyCollection<Duration> Sunshine1h { get; set; }
        
        [JsonProperty("precipitation10m", ItemConverterType = typeof(MillimeterLengthJsonConverter))]
        public IReadOnlyCollection<Length> Precipitation10m { get; set; }

        [JsonProperty("precipitationMin10m", ItemConverterType = typeof(MillimeterLengthJsonConverter))]
        public IReadOnlyCollection<Length> PrecipitationMin10m { get; set; }

        [JsonProperty("precipitationMax10m", ItemConverterType = typeof(MillimeterLengthJsonConverter))]
        public IReadOnlyCollection<Length> PrecipitationMax10m { get; set; }

        [JsonProperty("precipitation1h", ItemConverterType = typeof(MillimeterLengthJsonConverter))]
        public IReadOnlyCollection<Length> Precipitation1h { get; set; }

        [JsonProperty("precipitationMin1h", ItemConverterType = typeof(MillimeterLengthJsonConverter))]
        public IReadOnlyCollection<Length> PrecipitationMin1h { get; set; }

        [JsonProperty("precipitationMax1h", ItemConverterType = typeof(MillimeterLengthJsonConverter))]
        public IReadOnlyCollection<Length> PrecipitationMax1h { get; set; }
        
        [JsonProperty("precipitationProbability3h", ItemConverterType = typeof(PercentRatioJsonConverter))]
        public IReadOnlyCollection<Ratio> PrecipitationProbability3h { get; set; }

        public override string ToString()
        {
            return $"{this.Start}";
        }
    }
}