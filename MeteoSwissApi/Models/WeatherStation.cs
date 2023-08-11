using System.Diagnostics;
using UnitsNet;

namespace MeteoSwissApi.Models
{
    [DebuggerDisplay("Station \"{this.StationCode}\"")]
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
        /// Station height above sea level.
        /// </summary>
        public Length? Altitude { get; set; }

        /// <summary>
        /// Barometric altitude above ground.
        /// </summary>
        public Length? BarometricAltitude { get; set; }

        /// <summary>
        /// Latitude of GPS position.
        /// </summary>
        public string Latitude { get; set; }

        /// <summary>
        /// Longitude of GPS position.
        /// </summary>
        public string Longitude { get; set; }
        
        /// <summary>
        /// Political canton.
        /// </summary>
        public string Canton { get; set; }
    }
}