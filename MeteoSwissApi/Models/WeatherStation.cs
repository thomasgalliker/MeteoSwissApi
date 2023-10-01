using System;
using System.Collections.Generic;
using UnitsNet;

namespace MeteoSwissApi.Models
{
    /// <summary>
    /// SwissMetNet weather station.
    /// </summary>
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

        /// <summary>
        /// WIGOS station identifier.
        /// </summary>
        public string WigosId { get; set; }

        public WeatherStationType StationType { get; set; }

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
        /// Owner(s) of the data provided by the weather station.
        /// </summary>
        public IReadOnlyCollection<string> DataOwners { get; set; } = Array.Empty<string>();

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