using Newtonsoft.Json;

namespace MeteoSwissApi.Models
{
    public class SlfStationMeasurement
    {
        [JsonIgnore]
        public SlfStation Station { get; set; }

        [JsonProperty("heightNewSnow")]
        public SlfStationMeasurementLengthValue NewSnowHeight1d { get; set; }

        [JsonProperty("heightNewSnow3d")]
        public SlfStationMeasurementLengthValue NewSnowHeight3d { get; set; }

        [JsonProperty("heightNewSnow7d")]
        public SlfStationMeasurementLengthValue NewSnowHeight7d { get; set; }

        [JsonProperty("snowHeight")]
        public SlfStationMeasurementLengthValue SnowHeight { get; set; }

        [JsonProperty("temperatureAir")]
        public SlfStationMeasurementTemperatureValue AirTemperature { get; set; }

        [JsonProperty("temperatureSnowSurface")]
        public SlfStationMeasurementTemperatureValue SurfaceTemperature { get; set; }
        
        [JsonProperty("windVelocityMean")]
        public SlfStationMeasurementSpeedValue WindSpeedMean { get; set; }
        
        [JsonProperty("windDirectionMean")]
        public SlfStationMeasurementAngleValue WindDirection { get; set; }
        
        [JsonProperty("windVelocityMax")]
        public SlfStationMeasurementSpeedValue WindSpeedMax { get; set; }

        public override string ToString()
        {
            return $"{this.Station?.Code}";
        }
    }
}