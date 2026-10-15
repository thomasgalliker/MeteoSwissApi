namespace MeteoSwissApi.Models.Csv
{
    /// <summary>
    /// Parses numbers from MeteoSwiss CSV files. The files always use a decimal point,
    /// so parsing must not depend on the current culture.
    /// </summary>
    internal static class CsvValueParser
    {
        internal static bool TryParseDecimal(string? text, out decimal value)
        {
            return decimal.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
        }

        internal static bool TryParseDouble(string? text, out double value)
        {
            return double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
        }
    }
}
