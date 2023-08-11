using System;
using System.Globalization;
using CsvHelper.Configuration;
using UnitsNet;

namespace MeteoSwissApi.Models.Csv
{
    internal class WeatherStationMeasurementCsvMapping : ClassMap<WeatherStationMeasurement>
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

            this.Map(m => m.AirTemperature).Convert(row =>
            {
                var columnValue = row.Row["tre200s0"];
                if (decimal.TryParse(columnValue, out var decimalValue))
                {
                    return Temperature.FromDegreesCelsius(decimalValue);
                }

                return null;
            });

            this.Map(m => m.Precipitation).Convert(row =>
            {
                var columnValue = row.Row["rre150z0"];
                if (decimal.TryParse(columnValue, out var length))
                {
                    return Length.FromMillimeters(length);
                }

                return Length.Zero;
            });

            this.Map(m => m.SunshineDuration).Convert(row =>
            {
                var columnValue = row.Row["sre000z0"];
                if (decimal.TryParse(columnValue, out var duration))
                {
                    return Duration.FromMinutes(duration);
                }

                return null;
            });
            
            this.Map(m => m.GlobalRadiation).Convert(row =>
            {
                var columnValue = row.Row["sre000z0"];
                if (decimal.TryParse(columnValue, out var duration))
                {
                    return Duration.FromMinutes(duration);
                }

                return null;
            });

            this.Map(m => m.RelativeAirHumidity).Convert(row =>
            {
                var columnValue = row.Row["ure200s0"];
                if (decimal.TryParse(columnValue, out var decimalValue))
                {
                    return RelativeHumidity.FromPercent(decimalValue);
                }

                return null;
            });

            this.Map(m => m.WindDirection).Convert(row =>
            {
                var columnValue = row.Row["dkl010z0"];
                if (decimal.TryParse(columnValue, out var decimalValue))
                {
                    return Angle.FromDegrees(decimalValue);
                }

                return null;
            });

            this.Map(m => m.WindSpeed).Convert(row =>
            {
                var columnValue = row.Row["fu3010z0"];
                if (decimal.TryParse(columnValue, out var decimalValue))
                {
                    return Speed.FromKilometersPerHour(decimalValue);
                }

                return null;
            });
            this.Map(m => m.PressureQFE).Convert(row =>
            {
                var columnValue = row.Row["prestas0"];
                if (decimal.TryParse(columnValue, out var decimalValue))
                {
                    return Pressure.FromHectopascals(decimalValue);
                }

                return null;
            });
            this.Map(m => m.PressureQFF).Convert(row =>
            {
                var columnValue = row.Row["pp0qffs0"];
                if (decimal.TryParse(columnValue, out var decimalValue))
                {
                    return Pressure.FromHectopascals(decimalValue);
                }

                return null;
            });
            this.Map(m => m.PressureQNH).Convert(row =>
            {
                var columnValue = row.Row["pp0qnhs0"];
                if (decimal.TryParse(columnValue, out var decimalValue))
                {
                    return Pressure.FromHectopascals(decimalValue);
                }

                return null;
            });
        }
    }


    //        type StationData struct {

    //    Station string      `csv:"Station/Location"`
    //	DateTime DateTime    `csv:"Date"`
    //	AirTemperature NullFloat64 `csv:"tre200s0"` // °C: Air temperature 2 m above ground; current value
    //	Precipitation NullFloat64 `csv:"rre150z0"` // mm: Precipitation; current value
    //	SunshineDuration NullFloat64 `csv:"sre000z0"` //min: Sunshine duration; ten minutes total
    //	GlobalRadiation NullFloat64 `csv:"gre000z0"` // W/m²: Global radiation; ten minutes mean
    //	RelativeAirHumidity NullFloat64 `csv:"ure200s0"` // %: Relative air humidity 2 m above ground; current value
    //	DewPointTemperature NullFloat64 `csv:"tde200s0"` // °C: Dew point temperature 2 m above ground; current value
    //	WindDirection NullFloat64 `csv:"dkl010z0"` // °: wind direction; ten minutes mean
    //	WindSpeed NullFloat64 `csv:"fu3010z0"` // km/h: Wind speed; ten minutes mean
    //	GustPeak NullFloat64 `csv:"fu3010z1"` // km/h: Gust peak (one second); maximum
    //	PressureQFE NullFloat64 `csv:"prestas0"` // hPa: Pressure at station level (QFE); current value
    //	PressureQFF NullFloat64 `csv:"pp0qffs0"` // hPa: Pressure reduced to sea level (QFF); current value
    //	PressureQNH NullFloat64 `csv:"pp0qnhs0"` // hPa: Pressure reduced to sea level according to standard atmosphere (QNH); current value
    //	GeopotentialHeight850 NullFloat64 `csv:"ppz850s0"` // gpm: geopotential height of the 850 hPa-surface; current value
    //	GeopotentialHeight700 NullFloat64 `csv:"ppz700s0"` // gpm: geopotential height of the 700 hPa-surface; current value
    //	WindDirectionVectorial NullFloat64 `csv:"dv1towz0"` // °: wind direction vectorial, average of 10 min; instrument 1
    //	WindSpeedTower NullFloat64 `csv:"fu3towz0"` // km/h: Wind speed tower; ten minutes mean
    //	GustPeakTower NullFloat64 `csv:"fu3towz1"` // km/h: Gust peak (one second) tower; maximum
    //	AirTemperatureTool NullFloat64 `csv:"ta1tows0"` // °C: Air temperature tool 1
    //	RelativeAirHumidityTower NullFloat64 `csv:"uretows0"` // %: Relative air humidity tower; current value
    //	DewPointTower NullFloat64 `csv:"tdetows0"` // °C: Dew point tower
    //}



    //        Legend for VQHA80
    //Station/Location;yyyyMMddHHmm;tre200s0;rre150z0;sre000z0;gre000z0;ure200s0;tde200s0;dkl010z0;fu3010z0;fu3010z1;prestas0;pp0qffs0;pp0qnhs0;ppz850s0;ppz700s0;dv1towz0;fu3towz0;fu3towz1;ta1tows0;uretows0;tdetows0

    //Station/Location https://data.geo.admin.ch/ch.meteoschweiz.messnetz-automatisch/ch.meteoschweiz.messnetz-automatisch_en.csv

    //Time in UTC : 00:40 UTC = 02:40 Summer Time
    //                        = 01:40 Winter Time

    //Missing readings        -

    //Parameter Unit           Description
    //tre200s0       °C Air temperature 2 m above ground; current value
    //rre150z0 mm             Precipitation; ten minutes total
    //sre000z0       min Sunshine duration; ten minutes total
    //gre000z0       W/m²           Global radiation; ten minutes mean
    //ure200s0       %              Relative air humidity 2 m above ground; current value
    //tde200s0       °C Dew point 2 m above ground; current value
    //dkl010z0       °              wind direction; ten minutes mean
    //fu3010z0       km/h Wind speed; ten minutes mean
    //fu3010z1       km/h Gust peak(one second); maximum
    //prestas0       hPa Pressure at station level(QFE); current value
    //pp0qffs0 hPa            Pressure reduced to sea level(QFF); current value
    //pp0qnhs0 hPa            Pressure reduced to sea level according to standard atmosphere(QNH); current value
    //ppz850s0 gpm            geopotential height of the 850 hPa-surface; current value
    //ppz700s0 gpm            geopotential height of the 700 hPa-surface; current value
    //dv1towz0       °              wind direction vectorial, average of 10 min; instrument 1
    //fu3towz0 km/h Wind speed tower; ten minutes mean
    //fu3towz1       km/h Gust peak(one second) tower; maximum
    //ta1tows0       °C Air temperature tool 1
    //uretows0       %              Relative air humidity tower; current value
    //tdetows0       °C Dew point tower


}