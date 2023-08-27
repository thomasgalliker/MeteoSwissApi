using System.Diagnostics;
using UnitsNet;

namespace MeteoSwissApi.Models
{
    [DebuggerDisplay("{this.StationCode}")]
    public class WeatherStation
    {
        /// <summary>
        /// The name of the place where the weather station is positioned.
        /// </summary>
        public string Place { get; set; }

        /// <summary>
        /// Internal station code, also referred to as 'Abbreviation'.
        /// </summary>
        public string StationCode { get; set; }
        
        public string WigosId { get; set; }
        
        public string StationType { get; set; }

        /// <summary>
        /// Barometric altitude above ground.
        /// </summary>
        /// <remarks>
        /// See <see cref="Location"/> (property <see cref="GeoCoordinate.Altitude"/>) 
        /// for station height (in meters a.s.l.).
        /// </remarks>
        public Length? BarometricAltitude { get; set; }

        /// <summary>
        /// Geolocation of the weather station.
        /// </summary>
        public GeoCoordinate Location { get; set; }

        /// <summary>
        /// Political canton.
        /// </summary>
        public string Canton { get; set; }

        public override string ToString()
        {
            return this.StationCode;
        }
    }
}