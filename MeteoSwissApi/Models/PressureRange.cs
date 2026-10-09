namespace MeteoSwissApi.Models
{
    public class PressureRange : Range<Pressure>, IFormattable
    {
        public static readonly PressureRange VeryLow = new PressureRange(nameof(VeryLow), min: Pressure.FromHectopascals(0), max: Pressure.FromHectopascals(998), minInclusive: true, maxInclusive: true);
        public static readonly PressureRange Low = new PressureRange(nameof(Low), min: Pressure.FromHectopascals(998), max: Pressure.FromHectopascals(1008), minInclusive: false, maxInclusive: false);
        public static readonly PressureRange Average = new PressureRange(nameof(Average), min: Pressure.FromHectopascals(1008), max: Pressure.FromHectopascals(1018), minInclusive: true, maxInclusive: true);
        public static readonly PressureRange High = new PressureRange(nameof(High), min: Pressure.FromHectopascals(1018), max: Pressure.FromHectopascals(1028), minInclusive: false, maxInclusive: false);
        public static readonly PressureRange VeryHigh = new PressureRange(nameof(VeryHigh), min: Pressure.FromHectopascals(1028), max: Pressure.FromHectopascals(int.MaxValue), minInclusive: true, maxInclusive: true);

        /// <summary>
        /// The pressure ranges from <see cref="VeryLow"/> to <see cref="VeryHigh"/>.
        /// </summary>
        public static readonly IReadOnlyList<PressureRange> All = Array.AsReadOnly(new[]
        {
            VeryLow,
            Low,
            Average,
            High,
            VeryHigh,
        });

        private readonly string resourceId;

        private PressureRange(string resourceId, Pressure min, Pressure max, bool minInclusive, bool maxInclusive)
            : base(min, max, minInclusive, maxInclusive)
        {
            this.resourceId = resourceId;
        }

        /// <summary>
        /// Returns the range that contains <paramref name="value"/>.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> is negative.</exception>
        public static PressureRange FromValue(Pressure value)
        {
            foreach (var pressureRange in All)
            {
                if (pressureRange.InRange(value))
                {
                    return pressureRange;
                }
            }

            throw new ArgumentOutOfRangeException(nameof(value), value, "Pressure must not be negative.");
        }

        public override string ToString()
        {
            return this.ToString("N", null);
        }

        public string ToString(string? format)
        {
            return this.ToString(format, null);
        }

        /// <summary>
        /// Formats the range: "N" (default) returns its name, such as "Very low", in the language of
        /// <paramref name="formatProvider"/> if that is a <see cref="CultureInfo"/>, otherwise of the current UI culture.
        /// "I" returns the interval, such as "[0 hPa, 998 hPa]", with the bounds formatted by <paramref name="formatProvider"/>.
        /// </summary>
        public string ToString(string? format, IFormatProvider? formatProvider)
        {
            if (string.IsNullOrEmpty(format))
            {
                format = "N";
            }

            switch (format)
            {
                case "I":
                    return base.ToString(formatProvider);
                case "N":
                default:
                    var text = PressureRanges.ResourceManager.GetString(this.resourceId, formatProvider as CultureInfo);
                    return text ?? this.resourceId;
            }
        }
    }
}