namespace MeteoSwissApi.Models
{
    [DebuggerDisplay("{this.Location}")]
    public class SlfLocation
    {
        private List<double> coordinates = new List<double>();

        [JsonProperty("type")]
        public string Type { get; set; } = null!;

        /// <summary>
        /// The GeoJSON coordinates: longitude, latitude and, if present, the altitude in meters. Setting them sets <see cref="Location"/>.
        /// </summary>
        [JsonInclude]
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

                    var longitude = value.ElementAtOrDefault(0);
                    var latitude = value.ElementAtOrDefault(1);

                    if (value.Count >= 2 && GeoCoordinate.IsValidLatitude(latitude) && GeoCoordinate.IsValidLongitude(longitude))
                    {
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

        [JsonIgnore]
        public GeoCoordinate? Location { get; private set; }
    }
}
