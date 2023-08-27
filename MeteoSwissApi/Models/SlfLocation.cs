using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnitsNet;

namespace MeteoSwissApi.Models
{
    [DebuggerDisplay("{this.Location}")]
    public class SlfLocation
    {
        private List<double> coordinates = new List<double>();

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("coordinates")]
        internal List<double> Coordinates
        {
            get => this.coordinates;
            set
            {
                if (this.coordinates != value)
                {
                    if (value == null)
                    {
                        this.Location = null;
                    }

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
                }
            }
        }

        public GeoCoordinate Location { get; private set; }
    }
}
