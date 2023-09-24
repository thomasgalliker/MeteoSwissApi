using System;
using System.Collections.Generic;
using System.Globalization;
using MeteoSwissApi.Resources.Strings;
using UnitsNet;

namespace MeteoSwissApi.Models
{
    public class HumidityRange : Range<RelativeHumidity>, IFormattable
    {
        public static readonly HumidityRange VeryDry = new HumidityRange(nameof(VeryDry), min: RelativeHumidity.FromPercent(0), max: RelativeHumidity.FromPercent(30), minInclusive: true, maxInclusive: true);
        public static readonly HumidityRange Dry = new HumidityRange(nameof(Dry), min: RelativeHumidity.FromPercent(30), max: RelativeHumidity.FromPercent(40), minInclusive: false, maxInclusive: false);
        public static readonly HumidityRange Average = new HumidityRange(nameof(Average), min: RelativeHumidity.FromPercent(40), max: RelativeHumidity.FromPercent(70), minInclusive: true, maxInclusive: true);
        public static readonly HumidityRange Moist = new HumidityRange(nameof(Moist), min: RelativeHumidity.FromPercent(70), max: RelativeHumidity.FromPercent(80), minInclusive: false, maxInclusive: false);
        public static readonly HumidityRange VeryMoist = new HumidityRange(nameof(VeryMoist), min: RelativeHumidity.FromPercent(80), max: RelativeHumidity.FromPercent(100), minInclusive: true, maxInclusive: true);

        public static readonly IEnumerable<HumidityRange> All = new List<HumidityRange>
        {
            VeryDry,
            Dry,
            Average,
            Moist,
            VeryMoist
        };

        private readonly string resourceId;

        private HumidityRange(string resourceId, RelativeHumidity min, RelativeHumidity max, bool minInclusive, bool maxInclusive)
            : base(min, max, minInclusive, maxInclusive)
        {
            this.resourceId = resourceId;
        }

        public static HumidityRange FromValue(RelativeHumidity value)
        {
            foreach (var humidityRange in All)
            {
                if (humidityRange.InRange(value))
                {
                    return humidityRange;
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
                    var str = HumidityRanges.ResourceManager.GetString(this.resourceId, (CultureInfo)(provider ?? CultureInfo.CurrentCulture));
                    return str;
            }

        }
    }
}