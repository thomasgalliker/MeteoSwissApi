using System;
using System.Globalization;
using System.Linq;
using MeteoSwissApi.Resources.Strings;

namespace MeteoSwissApi.Models
{
    public readonly struct WarnLevel : IFormattable
    {
        public static readonly WarnLevel NoWarnLevel = new WarnLevel(0);
        public static readonly WarnLevel Level1 = new WarnLevel(1);
        public static readonly WarnLevel Level2 = new WarnLevel(2);
        public static readonly WarnLevel Level3 = new WarnLevel(3);
        public static readonly WarnLevel Level4 = new WarnLevel(4);
        public static readonly WarnLevel Level5 = new WarnLevel(5);

        public static readonly int MinValue = Level1;
        public static readonly int MaxValue = Level5;

        public static readonly WarnLevel[] All =
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
            if (level < MinValue)
            {
                return NoWarnLevel;
            }

            var warnLevel = All.SingleOrDefault(x => x.Level == level);
            if (warnLevel == default)
            {
                throw new ArgumentOutOfRangeException(nameof(level), $"Value must be between {MinValue} and {MaxValue}");
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

            provider ??= CultureInfo.CurrentCulture;

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