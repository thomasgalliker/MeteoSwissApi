using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MeteoSwissApi.Resources;

namespace MeteoSwissApi.Models
{
    public struct WarnLevel : IFormattable
    {
        public static readonly WarnLevel Level1 = new WarnLevel(1);
        public static readonly WarnLevel Level2 = new WarnLevel(2);
        public static readonly WarnLevel Level3 = new WarnLevel(3);
        public static readonly WarnLevel Level4 = new WarnLevel(4);
        public static readonly WarnLevel Level5 = new WarnLevel(5);

        public static readonly IEnumerable<WarnLevel> All = new List<WarnLevel>
        {
           Level1,
           Level2,
           Level3,
           Level4,
           Level5,
        };

        private WarnLevel(int level)
        {
            this.Level = level;
        }

        public int Level { get; }

        public static WarnLevel FromValue(int level)
        {
            var warnLevel = All.SingleOrDefault(x => x.Level == level);
            if (warnLevel == default)
            {
                throw new ArgumentOutOfRangeException(nameof(level), $"Value must be between {All.Min(x => x.Level)} and {All.Max(x => x.Level)}");
            }

            return warnLevel;
        }

        public static implicit operator WarnLevel(int v) => FromValue(v);

        public static implicit operator WarnLevel(long v) => FromValue((int)v);

        public static implicit operator int(WarnLevel h) => h.Level;

        public static implicit operator long(WarnLevel h) => h.Level;

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

            if (provider == null)
            {
                provider = CultureInfo.CurrentCulture;
            }

            switch (format)
            {
                case "N":
                    return $"{this.Level}";
                case "G":
                default:
                    var str = WarnLevels.ResourceManager.GetString($"Level{this.Level}", (CultureInfo)provider);
                    return str;
            }
        }
    }
}