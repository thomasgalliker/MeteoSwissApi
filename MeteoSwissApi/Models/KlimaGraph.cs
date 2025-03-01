using System;
using MeteoSwissApi.Models.Converters;
using Newtonsoft.Json;
using UnitsNet;

namespace MeteoSwissApi.Models
{
    public class KlimaGraph
    {
        [JsonProperty("plz")]
        public int Plz { get; set; }

        [JsonProperty("start")]
        [JsonConverter(typeof(EpochDateTimeConverter))]
        public DateTime Start { get; set; }

        [JsonProperty("temperatureGraphValues")]
        public TemperatureGraphValues TemperatureGraphValues { get; set; }

        [JsonProperty("sunshineGraphValues")]
        public SunshineGraphValues SunshineGraphValues { get; set; }

        [JsonProperty("precipitationGraphValues")]
        public PrecipitationGraphValues PrecipitationGraphValues { get; set; }
    }

    /// <summary>
    /// Long-term climate graph for temperature values.
    /// </summary>
    public class TemperatureGraphValues
    {
        /// <summary>
        /// Monthly absolute temperature values, last 24 months.
        /// </summary>
        [JsonProperty("abs", ItemConverterType = typeof(TemperatureJsonConverter))]
        public Temperature[] Absolute { get; set; }

        /// <summary>
        /// Monthly climate normal temperature values, last 24 months.
        /// </summary>
        /// <remarks>
        /// Climate normals help contextualize current weather conditions.
        /// They represent a 30-year average of temperature for the corresponding month.
        /// </remarks>
        [JsonProperty("norm", ItemConverterType = typeof(TemperatureJsonConverter))]
        public Temperature[] Normal { get; set; }

        /// <summary>
        /// Monthly temperature deviations, last 24 months.
        /// </summary>
        [JsonProperty("abweichung", ItemConverterType = typeof(TemperatureJsonConverter))]
        public Temperature[] Deviation { get; set; }
    }

    public class SunshineGraphValues
    {
        /// <summary>
        /// Monthly absolute sunshine duration values, last 24 months.
        /// </summary>
        [JsonProperty("abs", ItemConverterType = typeof(HourDurationJsonConverter))]
        public Duration[] Absolute { get; set; }

        /// <summary>
        /// Monthly climate normal sunshine duration values, last 24 months.
        /// </summary>
        /// <remarks>
        /// Climate normals help contextualize current weather conditions.
        /// They represent a 30-year average of sunshine duration for the corresponding month.
        /// </remarks>
        [JsonProperty("norm", ItemConverterType = typeof(HourDurationJsonConverter))]
        public Duration[] Normal { get; set; }

        /// <summary>
        /// Monthly sunshine duration deviations, last 24 months.
        /// </summary>
        [JsonProperty("abweichung", ItemConverterType = typeof(DecimalFractionRatioJsonConverter))]
        public Ratio[] Deviation { get; set; }
    }

    public class PrecipitationGraphValues
    {
        /// <summary>
        /// Monthly absolute precipitation values, last 24 months.
        /// </summary>
        [JsonProperty("abs", ItemConverterType = typeof(MillimeterLengthJsonConverter))]
        public Length[] Absolute { get; set; }

        /// <summary>
        /// Monthly climate normal precipitation values, last 24 months.
        /// </summary>
        /// <remarks>
        /// Climate normals help contextualize current weather conditions.
        /// They represent a 30-year average of precipitation for the corresponding month.
        /// </remarks>
        [JsonProperty("norm", ItemConverterType = typeof(MillimeterLengthJsonConverter))]
        public Length[] Normal { get; set; }

        /// <summary>
        /// Monthly precipitation deviations, last 24 months.
        /// </summary>
        [JsonProperty("abweichung", ItemConverterType = typeof(DecimalFractionRatioJsonConverter))]
        public Ratio[] Deviation { get; set; }
    }
}