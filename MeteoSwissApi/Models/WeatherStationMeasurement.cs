using System;
using System.Diagnostics;
using UnitsNet;

namespace MeteoSwissApi.Models
{
    [DebuggerDisplay("Station {this.StationCode}, {this.Date}, {this.AirTemperature}")]
    public class WeatherStationMeasurement
    {
        /// <inheritdoc cref="WeatherStation.StationCode"/>
        public string StationCode { get; set; }

        /// <summary>
        /// Time of measurement.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Air temperature 2m above ground.
        /// </summary>
        public Temperature? AirTemperature { get; set; }

        /// <summary>
        /// Precipitation; ten minutes total.
        /// </summary>
        public Length? Precipitation { get; set; }

        /// <summary>
        /// Sunshine duration; ten minutes total.
        /// </summary>
        public Duration? SunshineDuration { get; set; }
        
        public Duration? GlobalRadiation { get; set; }

        /// <summary>
        /// Relative air humidity 2m above ground.
        /// </summary>
        public RelativeHumidity? RelativeAirHumidity { get; set; }
        
        public Angle? WindDirection { get; set; }
        
        public Speed? WindSpeed { get; set; }

        /// <summary>
        /// Pressure at station level (QFE).
        /// </summary>
        public Pressure? PressureQFE { get; set; }

        /// <summary>
        /// Pressure reduced to sea level (QFF).
        /// </summary>
        public Pressure? PressureQFF { get; set; }

        /// <summary>
        /// Pressure reduced to sea level according to standard atmosphere (QNH).
        /// </summary>
        public Pressure? PressureQNH { get; set; }


    }
}