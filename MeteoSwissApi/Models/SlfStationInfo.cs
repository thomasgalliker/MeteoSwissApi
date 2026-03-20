namespace MeteoSwissApi.Models
{
    public class SlfStationInfo : SlfStation
    {
        public SlfStationInfo()
        {
            this.NearestStations = Array.Empty<NearestStation>();
        }

        [JsonProperty("winterplotAvailable")]
        public bool WinterplotAvailable { get; set; }

        [JsonProperty("nearestStations")]
        public NearestStation[] NearestStations { get; set; }
    }


}
