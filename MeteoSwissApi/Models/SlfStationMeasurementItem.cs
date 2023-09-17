using System;
using System.Diagnostics;
using UnitsNet;

namespace MeteoSwissApi.Models
{
    [DebuggerDisplay("{this.Date}")]
    public class SlfStationMeasurementItem
    {
        public DateTime Date { get; set; }

        public Temperature TemperatureAir { get; set; }

        public SlfWindInfo Wind { get; set; }
    }
}
