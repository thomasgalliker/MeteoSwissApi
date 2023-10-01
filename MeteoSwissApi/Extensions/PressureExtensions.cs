using MeteoSwissApi.Models;
using UnitsNet;

namespace MeteoSwissApi.Extensions
{
    public static class PressureExtensions
    {
        public static PressureRange GetRange(this Pressure pressure)
        {
            return PressureRange.FromValue(pressure);
        }
    }
}
