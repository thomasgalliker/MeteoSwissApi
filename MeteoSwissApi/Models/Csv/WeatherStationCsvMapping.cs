using CsvHelper.Configuration;
using UnitsNet;
using static System.Collections.Specialized.BitVector32;

namespace MeteoSwissApi.Models.Csv
{
    internal class WeatherStationCsvMapping : ClassMap<WeatherStation>
    {
        internal WeatherStationCsvMapping()
        {
            this.Map(m => m.Place).Name("Station");
            this.Map(m => m.StationCode).Name("Abbr.");
            this.Map(m => m.WigosId).Name("WIGOS-ID");
            this.Map(m => m.StationType).Name("Station type");
            this.Map(m => m.Altitude).Convert(row =>
            {
                var columnValue = row.Row["Station height m a. sea level"];
                if (decimal.TryParse(columnValue, out var length))
                {
                    return Length.FromMeters(length);
                }

                return null;
            });
            this.Map(m => m.BarometricAltitude).Convert(row =>
            {
                var columnValue = row.Row["Barometric altitude m a. ground"];
                if (decimal.TryParse(columnValue, out var length))
                {
                    return Length.FromMeters(length);
                }

                return null;
            });
            this.Map(m => m.Latitude).Name("Latitude");
            this.Map(m => m.Longitude).Name("Longitude");
            this.Map(m => m.Canton).Name("Canton");
        }
    }
}