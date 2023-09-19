using System;
using System.Collections.Generic;
using System.Globalization;
using MeteoSwissApi.Resources.Strings;
using UnitsNet;

namespace MeteoSwissApi.Models
{
    public class PressureRange : Range<Pressure>, IFormattable
    {
        public static readonly PressureRange VeryLow = new PressureRange(nameof(VeryLow), min: Pressure.FromHectopascals(0), max: Pressure.FromHectopascals(998), minInclusive: true, maxInclusive: true);
        public static readonly PressureRange Low = new PressureRange(nameof(Low), min: Pressure.FromHectopascals(998), max: Pressure.FromHectopascals(1008), minInclusive: false, maxInclusive: false);
        public static readonly PressureRange Average = new PressureRange(nameof(Average), min: Pressure.FromHectopascals(1008), max: Pressure.FromHectopascals(1018), minInclusive: true, maxInclusive: true);
        public static readonly PressureRange High = new PressureRange(nameof(High), min: Pressure.FromHectopascals(1018), max: Pressure.FromHectopascals(1028), minInclusive: false, maxInclusive: false);
        public static readonly PressureRange VeryHigh = new PressureRange(nameof(VeryHigh), min: Pressure.FromHectopascals(1028), max: Pressure.FromHectopascals(int.MaxValue), minInclusive: true, maxInclusive: true);

        public static readonly IEnumerable<PressureRange> All = new List<PressureRange>
        {
            VeryLow,
            Low,
            Average,
            High,
            VeryHigh
        };

        private readonly string resourceId;

        private PressureRange(string resourceId, Pressure min, Pressure max, bool minInclusive, bool maxInclusive)
            : base(min, max, minInclusive, maxInclusive)
        {
            this.resourceId = resourceId;
        }

        public static PressureRange FromValue(Pressure value)
        {
            foreach (var pressureRange in All)
            {
                if (pressureRange.InRange(value))
                {
                    return pressureRange;
                }
            }

            throw new ArgumentOutOfRangeException(nameof(value), "Value must be between 0 and 100");
        }

        public override string ToString()
        {
            return this.ToString("N", null);
        }

        public string ToString(string format)
        {
            return this.ToString(format, null);
        }

        public string ToString(string format, IFormatProvider provider)
        {
            if (string.IsNullOrEmpty(format))
            {
                format = "N";
            }

            switch (format)
            {
                case "I":
                    return base.ToString();
                case "N":
                default:
                    var str = PressureRanges.ResourceManager.GetString(this.resourceId, (CultureInfo)(provider ?? CultureInfo.CurrentCulture));
                    return str;
            }

        }
    }
}