using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

using CsvHelper;
using CsvHelper.Configuration;
using MeteoSwissApi.Models.Csv;

namespace MeteoSwissApi.Utils
{
    internal class CsvImporter : ICsvImporter
    {
        private static readonly Encoding DefaultEncoding = Encoding.UTF8;

        public static IEnumerable<T> Import<T>(string content, Encoding encoding = null, string delimiter = ";")
        {
            if (string.IsNullOrEmpty(content))
            {
                throw new ArgumentNullException(nameof(content));
            }

            if (encoding == null)
            {
                encoding = DefaultEncoding;
            }

            List<T> importedRecords;

            using (var textReader = new StringReader(content))
            {
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = delimiter,
                    Encoding = encoding,
                    MissingFieldFound = null,
                    //ShouldSkipRecord = new ShouldSkipRecord(x => x.Row.ColumnCount < (x.Row.Context.Reader.HeaderRecord?.Length ?? 0))
                };

                var csvReader = new CsvReader(textReader, config);
                csvReader.Context.RegisterClassMap<WeatherStationCsvMapping>();
                csvReader.Context.RegisterClassMap<WeatherStationMeasurementCsvMapping>();

                importedRecords = csvReader.GetRecords<T>().ToList();
            }

            return importedRecords;
        }
    }

    internal interface ICsvImporter
    {
    }
}