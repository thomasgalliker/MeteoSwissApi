namespace MeteoSwissApi.Models
{
    [DebuggerDisplay("{this.Location}")]
    public class SlfLocation
    {
        private List<double> coordinates = new List<double>();

        [JsonProperty("type")]
        public string Type { get; set; } = null!;

        [JsonProperty("coordinates")]
        internal List<double> Coordinates
        {
            get => this.coordinates;
            set
            {
                value ??= new List<double>();

                if (this.coordinates != value)
                {
                    this.coordinates = value;

                    if (value.Count >= 2)
                    {
                        var longitude = value.ElementAtOrDefault(0);
                        var latitude = value.ElementAtOrDefault(1);
                        var location = new GeoCoordinate(latitude, longitude);

                        if (value.Count >= 3)
                        {
                            var altitude = value.ElementAtOrDefault(2);
                            location.Altitude = Length.FromMeters(altitude);
                        }

                        this.Location = location;
                    }
                    else
                    {
                        this.Location = null;
                    }
                }
            }
        }

        public GeoCoordinate? Location { get; private set; }
    }
}
