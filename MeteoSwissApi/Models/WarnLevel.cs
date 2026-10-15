namespace MeteoSwissApi.Models
{
    public readonly struct WarnLevel : IEquatable<WarnLevel>, IComparable<WarnLevel>, IComparable, IFormattable
    {
        public static readonly WarnLevel NoWarnLevel = new WarnLevel(0);

        public static readonly WarnLevel Level1 = new WarnLevel(1);

        public static readonly WarnLevel Level2 = new WarnLevel(2);

        public static readonly WarnLevel Level3 = new WarnLevel(3);

        public static readonly WarnLevel Level4 = new WarnLevel(4);

        public static readonly WarnLevel Level5 = new WarnLevel(5);

        public static readonly int MinValue = Level1.Level;

        public static readonly int MaxValue = Level5.Level;

        /// <summary>
        /// The warn levels 1 to 5, in ascending order.
        /// </summary>
        public static readonly IReadOnlyList<WarnLevel> All = Array.AsReadOnly(new[]
        {
            Level1,
            Level2,
            Level3,
            Level4,
            Level5,
        });

        private WarnLevel(int level)
        {
            this.Level = level;
        }

        public int Level { get; }

        /// <summary>
        /// <c>true</c> if this is <see cref="NoWarnLevel"/> or one of the levels in <see cref="All"/>.
        /// </summary>
        internal bool IsKnown => this.Level >= NoWarnLevel.Level && this.Level <= MaxValue;

        /// <summary>
        /// Returns the warn level with the given <paramref name="level"/>.
        /// A level this version does not know, for example a level MeteoSwiss added later,
        /// returns an unknown warn level that keeps the level.
        /// </summary>
        internal static WarnLevel FromValueOrUnknown(int level)
        {
            if (TryGetFromValue(level, out var warnLevel))
            {
                return warnLevel;
            }

            return new WarnLevel(level);
        }

        /// <summary>
        /// Returns the warn level with the given <paramref name="level"/>: <see cref="NoWarnLevel"/> for 0, or one of <see cref="All"/>.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="level"/> is negative or above <see cref="MaxValue"/>.</exception>
        public static WarnLevel FromValue(int level)
        {
            if (TryGetFromValue(level, out var warnLevel))
            {
                return warnLevel;
            }

            throw new ArgumentOutOfRangeException(nameof(level), level, $"Value must be between {NoWarnLevel.Level} and {MaxValue}");
        }

        public static bool TryGetFromValue(int level, out WarnLevel warnLevel)
        {
            if (level == NoWarnLevel.Level)
            {
                warnLevel = NoWarnLevel;
                return true;
            }

            if (level < MinValue || level > MaxValue)
            {
                warnLevel = NoWarnLevel;
                return false;
            }

            warnLevel = All[level - MinValue];
            return true;
        }

        public static explicit operator WarnLevel(int level) => FromValue(level);

        public static implicit operator int(WarnLevel warnLevel) => warnLevel.Level;

        public bool Equals(WarnLevel other)
        {
            return this.Level == other.Level;
        }

        public override bool Equals(object? obj)
        {
            return obj is WarnLevel other && this.Equals(other);
        }

        public override int GetHashCode()
        {
            return this.Level;
        }

        public static bool operator ==(WarnLevel left, WarnLevel right) => left.Equals(right);

        public static bool operator !=(WarnLevel left, WarnLevel right) => !left.Equals(right);

        public int CompareTo(WarnLevel other)
        {
            return this.Level.CompareTo(other.Level);
        }

        public int CompareTo(object? obj)
        {
            if (obj is null)
            {
                return 1;
            }

            if (obj is WarnLevel other)
            {
                return this.CompareTo(other);
            }

            throw new ArgumentException($"Object must be of type {nameof(WarnLevel)}.", nameof(obj));
        }

        public static bool operator <(WarnLevel left, WarnLevel right) => left.CompareTo(right) < 0;

        public static bool operator <=(WarnLevel left, WarnLevel right) => left.CompareTo(right) <= 0;

        public static bool operator >(WarnLevel left, WarnLevel right) => left.CompareTo(right) > 0;

        public static bool operator >=(WarnLevel left, WarnLevel right) => left.CompareTo(right) >= 0;

        public override string ToString()
        {
            return this.ToString(null, null);
        }

        public string ToString(string? format)
        {
            return this.ToString(format, null);
        }

        /// <summary>
        /// Formats the warn level: "G" (default) returns its text, such as "Considerable", in the language of
        /// <paramref name="formatProvider"/> if that is a <see cref="CultureInfo"/>, otherwise of the current UI culture.
        /// "N" returns the level as number. An unknown level returns its number with every format.
        /// </summary>
        public string ToString(string? format, IFormatProvider? formatProvider)
        {
            if (string.IsNullOrEmpty(format))
            {
                format = "G";
            }

            switch (format)
            {
                case "N":
                    return this.Level.ToString(formatProvider);
                case "G":
                default:
                    var text = WarnLevels.ResourceManager.GetString($"Level{this.Level}", formatProvider as CultureInfo);
                    return text ?? this.Level.ToString(formatProvider);
            }
        }
    }
}