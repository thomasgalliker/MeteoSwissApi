namespace MeteoSwissApi.Models
{
    /// <summary>
    /// The latest measurement of a SwissMetNet weather station, from the file VQHA80.csv. The remarks of each value
    /// name its column in that file. A value is <c>null</c> if the station reported no reading ("-" in the file).
    /// </summary>
    [DebuggerDisplay("Station {this.StationCode}, {this.Date}, {this.AirTemperature}")]
    public class WeatherStationMeasurement
    {
        /// <inheritdoc cref="WeatherStation.StationCode"/>
        /// <remarks>Column <c>Station/Location</c>.</remarks>
        public string StationCode { get; set; } = null!;

        /// <summary>
        /// Time of measurement, in UTC.
        /// </summary>
        /// <remarks>Column <c>Date</c>, formatted as yyyyMMddHHmm.</remarks>
        public DateTime Date { get; set; }

        /// <summary>
        /// Air temperature 2m above ground; current value.
        /// </summary>
        /// <remarks>Column <c>tre200s0</c>, in °C.</remarks>
        public Temperature? AirTemperature { get; set; }

        /// <summary>
        /// Precipitation; ten minutes total, or <c>null</c> if the station reported no value.
        /// </summary>
        /// <remarks>Column <c>rre150z0</c>, in mm.</remarks>
        public Length? Precipitation { get; set; }

        /// <summary>
        /// Sunshine duration; ten minutes total.
        /// </summary>
        /// <remarks>Column <c>sre000z0</c>, in minutes.</remarks>
        public Duration? SunshineDuration { get; set; }

        /// <summary>
        /// Global radiation; ten minutes mean.
        /// </summary>
        /// <remarks>Column <c>gre000z0</c>, in W/m².</remarks>
        public Irradiance? GlobalRadiation { get; set; }

        /// <summary>
        /// Relative air humidity 2m above ground; current value.
        /// </summary>
        /// <remarks>Column <c>ure200s0</c>, in %.</remarks>
        public RelativeHumidity? RelativeAirHumidity { get; set; }

        /// <summary>
        /// Dew point 2m above ground; current value.
        /// </summary>
        /// <remarks>Column <c>tde200s0</c>, in °C.</remarks>
        public Temperature? DewPointTemperature { get; set; }

        /// <summary>
        /// The wind direction; ten minutes mean.
        /// </summary>
        /// <remarks>Column <c>dkl010z0</c>, in °.</remarks>
        public Angle? WindDirection { get; set; }

        /// <summary>
        /// The wind speed; ten minutes mean.
        /// </summary>
        /// <remarks>Column <c>fu3010z0</c>, in km/h.</remarks>
        public Speed? WindSpeed { get; set; }

        /// <summary>
        /// Pressure at station level (QFE); current value.
        /// </summary>
        /// <remarks>Column <c>prestas0</c>, in hPa.</remarks>
        public Pressure? PressureQFE { get; set; }

        /// <summary>
        /// Pressure reduced to sea level (QFF); current value.
        /// </summary>
        /// <remarks>Column <c>pp0qffs0</c>, in hPa.</remarks>
        public Pressure? PressureQFF { get; set; }

        /// <summary>
        /// Pressure reduced to sea level according to standard atmosphere (QNH); current value.
        /// </summary>
        /// <remarks>Column <c>pp0qnhs0</c>, in hPa.</remarks>
        public Pressure? PressureQNH { get; set; }

        /// <summary>
        /// Gust peak (one second); maximum.
        /// </summary>
        /// <remarks>Column <c>fu3010z1</c>, in km/h.</remarks>
        public Speed? GustPeak { get; set; }

        /// <summary>
        /// Geopotential height of the 850 hPa-surface; current value.
        /// </summary>
        /// <remarks>Column <c>ppz850s0</c>, in gpm.</remarks>
        public Length? GeopotentialHeight850 { get; set; }

        /// <summary>
        /// Geopotential height of the 700 hPa-surface; current value.
        /// </summary>
        /// <remarks>Column <c>ppz700s0</c>, in gpm.</remarks>
        public Length? GeopotentialHeight700 { get; set; }

        /// <summary>
        /// Wind direction vectorial, average of 10 min; instrument 1.
        /// </summary>
        /// <remarks>Column <c>dv1towz0</c>, in °.</remarks>
        public Angle? WindDirectionVectorial { get; set; }

        /// <summary>
        /// Wind speed tower; ten minutes mean.
        /// </summary>
        /// <remarks>Column <c>fu3towz0</c>, in km/h.</remarks>
        public Speed? WindSpeedTower { get; set; }

        /// <summary>
        /// Gust peak (one second) tower; maximum.
        /// </summary>
        /// <remarks>Column <c>fu3towz1</c>, in km/h.</remarks>
        public Speed? GustPeakTower { get; set; }

        /// <summary>
        /// Air temperature tower (instrument 1).
        /// </summary>
        /// <remarks>Column <c>ta1tows0</c>, in °C.</remarks>
        public Temperature? AirTemperatureTower { get; set; }

        /// <summary>
        /// Relative air humidity tower; current value.
        /// </summary>
        /// <remarks>Column <c>uretows0</c>, in %.</remarks>
        public RelativeHumidity? RelativeAirHumidityTower { get; set; }

        /// <summary>
        /// Dew point tower.
        /// </summary>
        /// <remarks>Column <c>tdetows0</c>, in °C.</remarks>
        public Temperature? DewPointTower { get; set; }
    }
}