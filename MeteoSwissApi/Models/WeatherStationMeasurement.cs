using System;
using System.Diagnostics;
using UnitsNet;

namespace MeteoSwissApi.Models
{
    [DebuggerDisplay("Station {this.Abbreviation}, {this.Date}, {this.AirTemperature}")]
    public class WeatherStationMeasurement
    {
        /// <inheritdoc cref="WeatherStation.Abbreviation"/>
        public string Abbreviation { get; set; }

        /// <summary>
        /// Time of measurement.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Air temperature 2m above ground; current value.
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
        /// Relative air humidity 2m above ground; current value.
        /// </summary>
        public RelativeHumidity? RelativeAirHumidity { get; set; }
        
        public Angle? WindDirection { get; set; }
        
        public decimal? WindSpeed { get; set; }


    }
}