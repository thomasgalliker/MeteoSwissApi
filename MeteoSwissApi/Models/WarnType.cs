namespace MeteoSwissApi.Models
{
    public readonly struct WarnType : IComparable, IComparable<WarnType>, IComparable<int>, IEquatable<WarnType>, IFormattable
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

        public static readonly WarnType Drought = new WarnType(nameof(Drought), 12);

        /// <summary>
        /// The known warn types, in ascending order of their value.
        /// </summary>
        public static readonly IReadOnlyList<WarnType> All = Array.AsReadOnly(new[]
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
            Drought,
        });

        private static readonly Dictionary<int, WarnType> KnownWarnTypes = All.ToDictionary(w => w.Value);

        /// <summary>
        /// The resource id of a known warn type, or <c>null</c> for a default or unknown warn type.
        /// </summary>
        private readonly string? resourceId;

        private WarnType(string? resourceId, int value)
        {
            this.resourceId = resourceId;
            this.Value = value;
        }

        public int Value { get; }

        /// <summary>
        /// <c>true</c> if this is one of the warn types in <see cref="All"/>.
        /// </summary>
        internal bool IsKnown => this.resourceId != null;

        /// <summary>
        /// Returns the warn type with the given <paramref name="value"/>.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">No warn type in <see cref="All"/> has <paramref name="value"/>.</exception>
        public static WarnType FromValue(int value)
        {
            if (TryGetFromValue(value, out var warnType))
            {
                return warnType;
            }

            throw new ArgumentOutOfRangeException(nameof(value), value, $"Value must be between {All[0].Value} and {All[All.Count - 1].Value}");
        }

        public static bool TryGetFromValue(int value, out WarnType warnType)
        {
            return KnownWarnTypes.TryGetValue(value, out warnType);
        }

        /// <summary>
        /// Returns the warn type with the given <paramref name="value"/>.
        /// A value this version does not know, for example a warn type MeteoSwiss added later,
        /// returns an unknown warn type that keeps the value.
        /// </summary>
        internal static WarnType FromValueOrUnknown(int value)
        {
            if (TryGetFromValue(value, out var warnType))
            {
                return warnType;
            }

            return new WarnType(null, value);
        }

        public int CompareTo(object? obj)
        {
            if (obj is null)
            {
                return 1;
            }

            if (obj is WarnType other)
            {
                return this.CompareTo(other);
            }

            if (obj is int value)
            {
                return this.CompareTo(value);
            }

            throw new ArgumentException($"Object must be of type {nameof(WarnType)} or {nameof(Int32)}.", nameof(obj));
        }

        public int CompareTo(WarnType other)
        {
            var comparison = this.Value.CompareTo(other.Value);
            return comparison != 0 ? comparison : string.CompareOrdinal(this.resourceId, other.resourceId);
        }

        public int CompareTo(int other)
        {
            return this.Value.CompareTo(other);
        }

        public bool Equals(WarnType other)
        {
            // The resource id tells a default WarnType apart from Wind, which also has the value 0.
            return this.Value.Equals(other.Value) &&
                   string.Equals(this.resourceId, other.resourceId, StringComparison.Ordinal);
        }

        public override bool Equals(object? obj)
        {
            return obj is WarnType other && this.Equals(other);
        }

        public override int GetHashCode()
        {
            return this.Value;
        }

        public static bool operator ==(WarnType left, WarnType right) => left.Equals(right);

        public static bool operator !=(WarnType left, WarnType right) => !left.Equals(right);

        public static bool operator <(WarnType left, WarnType right) => left.CompareTo(right) < 0;

        public static bool operator <=(WarnType left, WarnType right) => left.CompareTo(right) <= 0;

        public static bool operator >(WarnType left, WarnType right) => left.CompareTo(right) > 0;

        public static bool operator >=(WarnType left, WarnType right) => left.CompareTo(right) >= 0;

        public static explicit operator WarnType(int value) => FromValue(value);

        public static implicit operator int(WarnType warnType) => warnType.Value;

        public override string ToString()
        {
            return this.ToString(null, null);
        }

        public string ToString(string? format)
        {
            return this.ToString(format, null);
        }

        /// <summary>
        /// Formats the warn type: "G" (default) returns its text, such as "Thunderstorms", in the language of
        /// <paramref name="formatProvider"/> if that is a <see cref="CultureInfo"/>, otherwise of the current UI culture.
        /// Any other format returns the name of the warn type. An unknown warn type returns its value.
        /// </summary>
        public string ToString(string? format, IFormatProvider? formatProvider)
        {
            if (string.IsNullOrEmpty(format))
            {
                format = "G";
            }

            if (this.resourceId is null)
            {
                // A default or unknown warn type has no translation.
                return this.Value.ToString(formatProvider);
            }

            switch (format)
            {
                case "G":
                    var translation = WarnTypes.ResourceManager.GetString(this.resourceId, formatProvider as CultureInfo);
                    return translation ?? this.resourceId;
                default:
                    return this.resourceId;
            }
        }
    }
}