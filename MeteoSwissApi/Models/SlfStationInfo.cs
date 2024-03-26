using System.Collections.Generic;
using Newtonsoft.Json;

namespace MeteoSwissApi.Models
{
    public class SlfStationInfo : SlfStation
    {
        [JsonProperty("winterplotAvailable")]
        public bool WinterplotAvailable { get; set; }

        [JsonProperty("nearestStations")]
        public List<NearestStation> NearestStations { get; } = new List<NearestStation>();
    }


}
