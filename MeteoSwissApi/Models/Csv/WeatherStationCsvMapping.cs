using CsvHelper.Configuration;
using UnitsNet;

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

            this.Map(m => m.BarometricAltitude).Convert(row =>
            {
                return GetAltitudeLength(row, "Barometric altitude m a. ground");
            });

            this.Map(m => m.Location).Convert(row =>
            {
                var latitudeString = row.Row["Latitude"];
                var longitudeString = row.Row["Longitude"];
                if (double.TryParse(latitudeString, out var latitude) &&
                    double.TryParse(longitudeString, out var longitude))
                {
                    var location = new GeoCoordinate(latitude, longitude);
                    location.Altitude = GetAltitudeLength(row, "Station height m a. sea level");
                    return location;
                }

                return null;
            });

            this.Map(m => m.Canton).Name("Canton");
        }

        private static Length? GetAltitudeLength(CsvHelper.ConvertFromStringArgs row, string columnTitle)
        {
            var columnValue = row.Row[columnTitle];
            if (decimal.TryParse(columnValue, out var length))
            {
                return Length.FromMeters(length);
            }

            return null;
        }
    }
}