using MeteoSwissApi.Models;
using UnitsNet;

namespace MeteoSwissApi.Extensions
{
    public static class HumidityExtensions
    {
        public static HumidityRange GetRange(this RelativeHumidity relativeHumidity)
        {
            return HumidityRange.FromValue(relativeHumidity);
        }
    }
}
