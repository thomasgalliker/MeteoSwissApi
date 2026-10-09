namespace MeteoSwissApi.Models
{
    /// <summary>
    /// A range of values from <see cref="Min"/> to <see cref="Max"/>; each bound is either part of the range (inclusive) or not (exclusive).
    /// </summary>
    /// <remarks>
    /// See also:
    /// https://raw.githubusercontent.com/ImaMapleTree/MirenFalls/0a482e153d3353d45608cd936a02445ddcdf023c/MirenFalls/Internal/Utils/Range.cs
    /// https://github.com/dundich/Pay/blob/f06645ce1c57c4be33f783944965d3faaaad6b88/Libs/Maybe2/IRange.cs
    /// </remarks>
    public class Range<T> where T : IComparable<T>
    {
        private readonly bool minInclusive;
        private readonly bool maxInclusive;

        /// <summary>
        /// Creates a range that includes both bounds.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="min"/> or <paramref name="max"/> is null.</exception>
        public Range(T min, T max)
            : this(min, max, minInclusive: true, maxInclusive: true)
        {
        }

        /// <param name="min">The lower bound.</param>
        /// <param name="max">The upper bound.</param>
        /// <param name="minInclusive"><c>true</c> if <paramref name="min"/> is part of the range.</param>
        /// <param name="maxInclusive"><c>true</c> if <paramref name="max"/> is part of the range.</param>
        /// <exception cref="ArgumentNullException"><paramref name="min"/> or <paramref name="max"/> is null.</exception>
        public Range(T min, T max, bool minInclusive, bool maxInclusive)
        {
            if (min is null)
            {
                throw new ArgumentNullException(nameof(min));
            }

            if (max is null)
            {
                throw new ArgumentNullException(nameof(max));
            }

            this.Min = min;
            this.Max = max;
            this.minInclusive = minInclusive;
            this.maxInclusive = maxInclusive;
        }

        public T Min { get; }

        public T Max { get; }

        /// <summary>
        /// Returns <c>true</c> if <paramref name="value"/> is part of the range.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
        public bool InRange(T value)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var isGreaterThanMin = this.minInclusive ? value.CompareTo(this.Min) >= 0 : value.CompareTo(this.Min) > 0;
            var isLessThanMax = this.maxInclusive ? value.CompareTo(this.Max) <= 0 : value.CompareTo(this.Max) < 0;

            return isGreaterThanMin && isLessThanMax;
        }

        /// <summary>
        /// <c>true</c> if no value is part of the range: <see cref="Min"/> is greater than <see cref="Max"/>,
        /// or both are equal and one of the bounds is exclusive.
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                var comparison = this.Min.CompareTo(this.Max);
                return comparison > 0 || (comparison == 0 && !(this.minInclusive && this.maxInclusive));
            }
        }

        public override string ToString()
        {
            return this.ToString(null);
        }

        public string ToString(IFormatProvider? formatProvider)
        {
            var start = this.minInclusive ? "[" : "(";
            var end = this.maxInclusive ? "]" : ")";
            return start + Format(this.Min, formatProvider) + ", " + Format(this.Max, formatProvider) + end;
        }

        private static string Format(T bound, IFormatProvider? formatProvider)
        {
            return bound is IFormattable formattable ? formattable.ToString(null, formatProvider) : bound.ToString() ?? string.Empty;
        }
    }
}