using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MeteoSwissApi.Resources.Strings;

namespace MeteoSwissApi.Models
{
    [Serializable]
    public struct WarnType : IComparable, IComparable<WarnType>, IComparable<int>, IEquatable<WarnType>, IFormattable
    {
        public static readonly WarnType Wind = new WarnType(nameof(Wind), 0);
        public static readonly WarnType Thunderstorms = new WarnType(nameof(Thunderstorms), 1);
        public static readonly WarnType Rain = new WarnType(nameof(Rain), 2);
        public static readonly WarnType Snow = new WarnType(nameof(Snow), 3);
        public static readonly WarnType SlipperyRoads = new WarnType(nameof(SlipperyRoads), 4);
        public static readonly WarnType Frost = new WarnType(nameof(Frost), 5);
        public static readonly WarnType MassMovements = new WarnType(nameof(MassMovements), 6);
        public static readonly WarnType Heat = new WarnType(nameof(Heat), 7);
        public static readonly WarnType Avalanches = new WarnType(nameof(Avalanches), 8);
        public static readonly WarnType Earthquakes = new WarnType(nameof(Earthquakes), 9);
        public static readonly WarnType ForestFire = new WarnType(nameof(ForestFire), 10);
        public static readonly WarnType Floods = new WarnType(nameof(Floods), 11);

        public static readonly IEnumerable<WarnType> All = new List<WarnType>
        {
            Wind,
            Thunderstorms,
            Rain,
            Snow,
            SlipperyRoads,
            Frost,
            MassMovements,
            Heat,
            Avalanches,
            Earthquakes,
            ForestFire,
            Floods,
        };

        private readonly string resourceId;

        private WarnType(string resourceId, int value)
        {
            this.resourceId = resourceId;
            this.Value = value;
        }

        public int Value { get; private set; }

        public static WarnType FromValue(int value)
        {
            var warnType = All.Cast<WarnType?>().SingleOrDefault(x => x.Value == value);
            if (warnType == null)
            {
                throw new ArgumentOutOfRangeException(nameof(value), $"Value must be between {All.Min(x => x.Value)} and {All.Max(x => x.Value)}");
            }

            return warnType.Value;
        }

        public int CompareTo(object obj)
        {
            if (obj is WarnType)
            {
                return this.Value.CompareTo((int)obj);
            }

            return this.Value.CompareTo(obj);
        }

        public int CompareTo(WarnType other)
        {
            return this.Value.CompareTo(other.Value);
        }

        public int CompareTo(int other)
        {
            return this.Value.CompareTo(other);
        }

        public bool Equals(WarnType other)
        {
            return this.Value.Equals(other.Value);
        }

        public static implicit operator WarnType(int v) => FromValue(v);

        public static implicit operator WarnType(long v) => FromValue((int)v);

        public static implicit operator int(WarnType h) => h.Value;

        public static implicit operator long(WarnType h) => h.Value;

        public override string ToString()
        {
            return this.ToString(null, null);
        }

        public string ToString(WarnLevel warnLevel)
        {
            return this.ToString(null, warnLevel, null);
        }

        public string ToString(WarnLevel warnLevel, IFormatProvider provider)
        {
            return this.ToString(null, warnLevel, provider);
        }

        public string ToString(string format, WarnLevel warnLevel, IFormatProvider provider)
        {
            if (string.IsNullOrEmpty(format))
            {
                format = "G";
            }

            provider ??= CultureInfo.CurrentCulture;

            switch (format)
            {
                case "G":
                default:
                    var warnLevelTitle = WarnTypes.ResourceManager.GetString($"{this.resourceId}_Level{warnLevel.Level}", (CultureInfo)provider);
                    if (warnLevelTitle == null)
                    {
                        warnLevelTitle = WarnLevels.ResourceManager.GetString($"Level{warnLevel.Level}", (CultureInfo)provider);
                    }
                   
                    return warnLevelTitle;
            }

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
                    var translation = WarnTypes.ResourceManager.GetString(this.resourceId, (CultureInfo)provider);
                    return translation;
                case "D":
                    var description = WarnTypes.ResourceManager.GetString($"{this.resourceId}_Description", (CultureInfo)provider);
                    return description;
                default:
                    return this.resourceId;
            }
        }
    }
}