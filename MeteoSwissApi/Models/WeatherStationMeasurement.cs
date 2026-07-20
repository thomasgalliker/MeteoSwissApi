namespace MeteoSwissApi.Models
{
    [DebuggerDisplay("Station {this.StationCode}, {this.Date}, {this.AirTemperature}")]
    public class WeatherStationMeasurement
    {
        /// <inheritdoc cref="WeatherStation.StationCode"/>
        public string StationCode { get; set; } = null!;

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
        public Length Precipitation { get; set; }

        /// <summary>
        /// Sunshine duration; ten minutes total.
        /// </summary>
        public Duration? SunshineDuration { get; set; }

        /// <summary>
        /// Global radiation; ten minutes mean.
        /// </summary>
        public Irradiance? GlobalRadiation { get; set; }

        /// <summary>
        /// Relative air humidity 2m above ground.
        /// </summary>
        public RelativeHumidity? RelativeAirHumidity { get; set; }

        /// <summary>
        /// Dew point 2m above ground.
        /// </summary>
        public Temperature? DewPointTemperature { get; set; }

        /// <summary>
        /// The wind direction.
        /// </summary>
        public Angle? WindDirection { get; set; }

        /// <summary>
        /// The wind speed.
        /// </summary>
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

        /// <summary>
        /// Gust peak (one second); maximum.
        /// </summary>
        public Speed? GustPeak { get; set; }

        /// <summary>
        /// Geopotential height of the 850 hPa-surface.
        /// </summary>
        public Length? GeopotentialHeight850 { get; set; }

        /// <summary>
        /// Geopotential height of the 700 hPa-surface.
        /// </summary>
        public Length? GeopotentialHeight700 { get; set; }

        /// <summary>
        /// Wind direction vectorial, average of 10 min; instrument 1.
        /// </summary>
        public Angle? WindDirectionVectorial { get; set; }

        /// <summary>
        /// Wind speed tower; ten minutes mean.
        /// </summary>
        public Speed? WindSpeedTower { get; set; }

        /// <summary>
        /// Gust peak (one second) tower; maximum.
        /// </summary>
        public Speed? GustPeakTower { get; set; }

        /// <summary>
        /// Air temperature tower (instrument 1).
        /// </summary>
        public Temperature? AirTemperatureTower { get; set; }

        /// <summary>
        /// Relative air humidity tower.
        /// </summary>
        public RelativeHumidity? RelativeAirHumidityTower { get; set; }

        /// <summary>
        /// Dew point tower.
        /// </summary>
        public Temperature? DewPointTower { get; set; }
    }
}