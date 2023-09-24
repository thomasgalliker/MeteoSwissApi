using System;
using System.Diagnostics;
using System.Globalization;
using MeteoSwissApi.Resources.Strings;

namespace MeteoSwissApi.Models
{
    [DebuggerDisplay("{this.value}")]
    public struct SlfStationType : IFormattable
    {
        public const string SnowFlat = "SNOW_FLAT";
        public const string SnowSlope = "SNOW_SLOPE";
        public const string Wind = "WIND";
        public const string Special = "SPECIAL";
        public const string FlowCapt = "FLOWCAPT";
        public const string Pluvio = "PLUVIO";

        public static readonly SlfStationType[] All =
        {
            new SlfStationType(SnowFlat),
            new SlfStationType(SnowSlope),
            new SlfStationType(Wind),
            new SlfStationType(Special),
            new SlfStationType(FlowCapt),
            new SlfStationType(Pluvio),
        };

        private readonly string value;

        public SlfStationType(string value)
        {
            this.value = value;
        }

        public string Value => this.value;

        public static implicit operator string(SlfStationType u) => u.value;

        public static implicit operator SlfStationType(string n) => new SlfStationType(n);

        public override string ToString()
        {
            return this.ToString(null, null);
        }

        public string ToString(string format)
        {
            return this.ToString(format, null);
        }

        public string ToString(string format, IFormatProvider provider)
        {
            if (string.IsNullOrEmpty(format))
            {
                format = "G";
            }

            provider ??= CultureInfo.CurrentCulture;

            switch (format)
            {
                case "G":
                    var translation = SlfStationTypes.ResourceManager.GetString(this.value, (CultureInfo)provider);
                    return translation;
                default:
                    return this.value;
            }
        }
    }
}