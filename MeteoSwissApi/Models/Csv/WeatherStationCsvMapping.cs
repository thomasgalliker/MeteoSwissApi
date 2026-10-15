
namespace MeteoSwissApi.Models.Csv
{
    internal sealed class WeatherStationCsvMapping : ClassMap<WeatherStation>
    {
        private static readonly char[] DataOwnerSeparators = { ',' };

        internal WeatherStationCsvMapping(ILogger logger)
        {
            this.Map(m => m.Place).Name("Station");

            this.Map(m => m.StationCode).Name("Abbr.");

            this.Map(m => m.WigosId).Name("WIGOS-ID");

            this.Map(m => m.StationType).Convert(row =>
            {
                var stationTypeString = row.Row["Station type"];

                switch (stationTypeString)
                {
                    case "Weather station":
                        return WeatherStationType.WeatherStation;
                    case "Precipitation station":
                        return WeatherStationType.PrecipitationStation;
                    case null:
                        return WeatherStationType.Unknown;
                    default:
                        logger.LogDebug("No mapping from '{StationType}' to WeatherStationType found.", stationTypeString);
                        return WeatherStationType.Unknown;
                }
            });

            this.Map(m => m.BarometricAltitude).Convert(row =>
            {
                return GetAltitudeLength(row, "Barometric altitude m a. ground");
            });

            this.Map(m => m.Location).Convert(row =>
            {
                var latitudeString = row.Row["Latitude"];
                var longitudeString = row.Row["Longitude"];
                if (CsvValueParser.TryParseDouble(latitudeString, out var latitude) &&
                    CsvValueParser.TryParseDouble(longitudeString, out var longitude))
                {
                    var location = new GeoCoordinate(latitude, longitude);
                    location.Altitude = GetAltitudeLength(row, "Station height m a. sea level");
                    return location;
                }

                return null;
            });

            this.Map(m => m.DataOwners).Convert(row =>
            {
                var dataOwnersString = row.Row["Data Owner"];
                if (dataOwnersString is string s)
                {
                    return s.Split(DataOwnerSeparators, StringSplitOptions.RemoveEmptyEntries)
                        .Select(dataOwner => dataOwner.Trim())
                        .Where(dataOwner => dataOwner.Length > 0)
                        .ToArray();
                }
                else
                {
                    return Array.Empty<string>();
                }
            });

            this.Map(m => m.Canton).Name("Canton");
        }

        private static Length? GetAltitudeLength(CsvHelper.ConvertFromStringArgs row, string columnTitle)
        {
            var columnValue = row.Row[columnTitle];
            if (CsvValueParser.TryParseDecimal(columnValue, out var length))
            {
                return Length.FromMeters(length);
            }

            return null;
        }
    }
}