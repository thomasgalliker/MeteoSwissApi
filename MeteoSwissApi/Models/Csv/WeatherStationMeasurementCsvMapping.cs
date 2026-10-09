using System.Linq.Expressions;

namespace MeteoSwissApi.Models.Csv
{
    /// <summary>
    /// Maps the columns of the SwissMetNet file VQHA80.csv. The column codes are documented on <see cref="WeatherStationMeasurement"/>.
    /// </summary>
    internal sealed class WeatherStationMeasurementCsvMapping : ClassMap<WeatherStationMeasurement>
    {
        internal WeatherStationMeasurementCsvMapping()
        {
            this.Map(m => m.StationCode).Name("Station/Location");

            this.Map(m => m.Date).Convert(row =>
            {
                var columnValue = row.Row["Date"];
                if (DateTime.TryParseExact(columnValue, "yyyyMMddHHmm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                {
                    return DateTime.SpecifyKind(date, DateTimeKind.Utc);
                }

                return DateTime.MinValue;
            });

            this.MapQuantity(m => m.AirTemperature, "tre200s0", v => Temperature.FromDegreesCelsius(v));
            this.MapQuantity(m => m.Precipitation, "rre150z0", v => Length.FromMillimeters(v));
            this.MapQuantity(m => m.SunshineDuration, "sre000z0", v => Duration.FromMinutes(v));
            this.MapQuantity(m => m.GlobalRadiation, "gre000z0", v => Irradiance.FromWattsPerSquareMeter(v));
            this.MapQuantity(m => m.RelativeAirHumidity, "ure200s0", v => RelativeHumidity.FromPercent(v));
            this.MapQuantity(m => m.WindDirection, "dkl010z0", v => Angle.FromDegrees(v));
            this.MapQuantity(m => m.WindSpeed, "fu3010z0", v => Speed.FromKilometersPerHour(v));
            this.MapQuantity(m => m.PressureQFE, "prestas0", v => Pressure.FromHectopascals(v));
            this.MapQuantity(m => m.PressureQFF, "pp0qffs0", v => Pressure.FromHectopascals(v));
            this.MapQuantity(m => m.PressureQNH, "pp0qnhs0", v => Pressure.FromHectopascals(v));
            this.MapQuantity(m => m.DewPointTemperature, "tde200s0", v => Temperature.FromDegreesCelsius(v));
            this.MapQuantity(m => m.GustPeak, "fu3010z1", v => Speed.FromKilometersPerHour(v));
            this.MapQuantity(m => m.GeopotentialHeight850, "ppz850s0", v => Length.FromMeters(v));
            this.MapQuantity(m => m.GeopotentialHeight700, "ppz700s0", v => Length.FromMeters(v));
            this.MapQuantity(m => m.WindDirectionVectorial, "dv1towz0", v => Angle.FromDegrees(v));
            this.MapQuantity(m => m.WindSpeedTower, "fu3towz0", v => Speed.FromKilometersPerHour(v));
            this.MapQuantity(m => m.GustPeakTower, "fu3towz1", v => Speed.FromKilometersPerHour(v));
            this.MapQuantity(m => m.AirTemperatureTower, "ta1tows0", v => Temperature.FromDegreesCelsius(v));
            this.MapQuantity(m => m.RelativeAirHumidityTower, "uretows0", v => RelativeHumidity.FromPercent(v));
            this.MapQuantity(m => m.DewPointTower, "tdetows0", v => Temperature.FromDegreesCelsius(v));
        }

        /// <summary>
        /// Maps a column to a nullable quantity. Values that cannot be parsed, such as the
        /// placeholder "-" for a missing reading, are mapped to <c>null</c>.
        /// </summary>
        private void MapQuantity<TQuantity>(
            Expression<Func<WeatherStationMeasurement, TQuantity?>> member,
            string columnName,
            Func<decimal, TQuantity> createQuantity)
            where TQuantity : struct
        {
            this.Map(member).Convert(row =>
            {
                var columnValue = row.Row[columnName];
                if (CsvValueParser.TryParseDecimal(columnValue, out var decimalValue))
                {
                    return createQuantity(decimalValue);
                }

                return null;
            });
        }
    }
}