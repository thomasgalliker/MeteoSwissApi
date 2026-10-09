using CsvHelper;
using MeteoSwissApi.Models.Csv;

namespace MeteoSwissApi.Utils
{
    internal static class CsvImporter
    {
        private const string Delimiter = ";";

        /// <summary>
        /// Reads the records of the semicolon-separated <paramref name="content"/>.
        /// </summary>
        /// <param name="content">The CSV content.</param>
        /// <param name="logger">Logs values the mappings cannot interpret.</param>
        /// <exception cref="ArgumentNullException"><paramref name="content"/> is null.</exception>
        /// <exception cref="CsvHelperException"><paramref name="content"/> cannot be read as <typeparamref name="T"/>.</exception>
        public static IReadOnlyList<T> Import<T>(string content, ILogger logger)
        {
            if (content is null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            using (var textReader = new StringReader(content))
            {
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = Delimiter,
                    MissingFieldFound = null,

                    // The stations file ends with metadata lines ("creation_time", "license", ...)
                    // which have fewer fields than the header. They are no records.
                    ShouldSkipRecord = args => args.Row.HeaderRecord is string[] header && args.Row.Parser.Count < header.Length,
                };

                var csvReader = new CsvReader(textReader, config);
                csvReader.Context.RegisterClassMap(new WeatherStationCsvMapping(logger));
                csvReader.Context.RegisterClassMap(new WeatherStationMeasurementCsvMapping());

                return csvReader.GetRecords<T>().ToList();
            }
        }
    }
}
