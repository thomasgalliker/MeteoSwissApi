using System.ComponentModel;

namespace MeteoSwissApi.Models
{
    public enum TemperatureUnit
    {
        [Description("K")]
        Kelvin = 0,

        [Description("°C")]
        Celsius,

        [Description("°F")]
        Fahrenheit,
    }
}