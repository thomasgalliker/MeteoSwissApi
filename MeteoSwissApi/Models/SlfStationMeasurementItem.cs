using System;
using MeteoSwissApi.Models.Converters;
using Newtonsoft.Json;
using UnitsNet;

namespace MeteoSwissApi.Models
{
    public abstract class SlfStationMeasurementItem<TValue>
    {
        [JsonProperty("timestamp")]
        public DateTime Date { get; set; }

        public abstract TValue Value { get; set; }

        public override string ToString()
        {
            return $"{{{this.Date}, {this.Value}}}";
        }
    }

    public class SlfStationMeasurementLengthValue : SlfStationMeasurementItem<Length>
    {
        [JsonProperty("value")]
        [JsonConverter(typeof(CentimeterLengthJsonConverter))]
        public override Length Value { get; set; }
    }

    public class SlfStationMeasurementTemperatureValue : SlfStationMeasurementItem<Temperature>
    {
        [JsonProperty("value")]
        [JsonConverter(typeof(TemperatureJsonConverter))]
        public override Temperature Value { get; set; }
    }

    public class SlfStationMeasurementSpeedValue : SlfStationMeasurementItem<Speed>
    {
        [JsonProperty("value")]
        [JsonConverter(typeof(SpeedJsonConverter))]
        public override Speed Value { get; set; }
    }

    public class SlfStationMeasurementAngleValue : SlfStationMeasurementItem<Angle>
    {
        [JsonProperty("value")]
        [JsonConverter(typeof(DegreeAngleJsonConverter))]
        public override Angle Value { get; set; }
    }
}