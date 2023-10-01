using System.Collections.Generic;
using Newtonsoft.Json;

namespace MeteoSwissApi.Models
{
    internal class SlfStationMeasurementsResponse
    {
        [JsonProperty("temperatureAir")]
        public List<SlfStationDateTemperature> TemperatureAir { get; set; }

        [JsonProperty("windVelocityMax")]
        public List<SlfStationDateSpeed> WindVelocityMax { get; set; }

        [JsonProperty("windVelocityMean")]
        public List<SlfStationDateSpeed> WindVelocityMean { get; set; }

        [JsonProperty("windDirectionMean")]
        public List<SlfStationDateAngle> WindDirectionMean { get; set; }
    }
}
