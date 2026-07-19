using System.Text.Json;

namespace MeteoSwissApi.Models.Converters
{
    /// <summary>
    /// Reads a temperature value from JSON and maps the MeteoSwiss "no data" sentinel
    /// (<c>32767</c> = <see cref="short.MaxValue"/>) to <c>null</c> instead of returning it
    /// as a valid temperature.
    /// </summary>
    internal class NullableTemperatureJsonConverter : TemperatureJsonConverterBase<Temperature?>
    {
        /// <summary>
        /// MeteoSwiss encodes missing/unavailable measurements as 32767 (0x7FFF)
        /// in the JSON API (e.g. a frozen <c>currentWeather</c> block reports <c>temperature: 32767</c>).
        /// </summary>
        private const double NoDataValue = 32767d;

        public override Temperature? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (TryReadDegreesCelsius(ref reader, out var celsius) && Math.Abs(celsius) < NoDataValue)
            {
                return Temperature.FromDegreesCelsius(celsius);
            }

            return null;
        }

        public override void Write(Utf8JsonWriter writer, Temperature? value, JsonSerializerOptions options)
        {
            if (value is Temperature temperature)
            {
                WriteTemperature(writer, temperature);
            }
            else
            {
                writer.WriteNullValue();
            }
        }
    }
}
