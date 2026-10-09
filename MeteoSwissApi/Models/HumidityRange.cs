namespace MeteoSwissApi.Models
{
    public class HumidityRange : Range<RelativeHumidity>, IFormattable
    {
        public static readonly HumidityRange VeryDry = new HumidityRange(nameof(VeryDry), min: RelativeHumidity.FromPercent(0), max: RelativeHumidity.FromPercent(30), minInclusive: true, maxInclusive: true);
        public static readonly HumidityRange Dry = new HumidityRange(nameof(Dry), min: RelativeHumidity.FromPercent(30), max: RelativeHumidity.FromPercent(40), minInclusive: false, maxInclusive: false);
        public static readonly HumidityRange Average = new HumidityRange(nameof(Average), min: RelativeHumidity.FromPercent(40), max: RelativeHumidity.FromPercent(70), minInclusive: true, maxInclusive: true);
        public static readonly HumidityRange Moist = new HumidityRange(nameof(Moist), min: RelativeHumidity.FromPercent(70), max: RelativeHumidity.FromPercent(80), minInclusive: false, maxInclusive: false);
        public static readonly HumidityRange VeryMoist = new HumidityRange(nameof(VeryMoist), min: RelativeHumidity.FromPercent(80), max: RelativeHumidity.FromPercent(100), minInclusive: true, maxInclusive: true);

        /// <summary>
        /// The humidity ranges from <see cref="VeryDry"/> to <see cref="VeryMoist"/>.
        /// </summary>
        public static readonly IReadOnlyList<HumidityRange> All = Array.AsReadOnly(new[]
        {
            VeryDry,
            Dry,
            Average,
            Moist,
            VeryMoist,
        });

        private readonly string resourceId;

        private HumidityRange(string resourceId, RelativeHumidity min, RelativeHumidity max, bool minInclusive, bool maxInclusive)
            : base(min, max, minInclusive, maxInclusive)
        {
            this.resourceId = resourceId;
        }

        /// <summary>
        /// Returns the range that contains <paramref name="value"/>. A sensor may read slightly more than 100 %,
        /// which counts as <see cref="VeryMoist"/>.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> is negative.</exception>
        public static HumidityRange FromValue(RelativeHumidity value)
        {
            if (value > VeryMoist.Max)
            {
                return VeryMoist;
            }

            foreach (var humidityRange in All)
            {
                if (humidityRange.InRange(value))
                {
                    return humidityRange;
                }
            }

            throw new ArgumentOutOfRangeException(nameof(value), value, "Relative humidity must not be negative.");
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
        /// Formats the range: "N" (default) returns its name, such as "Very dry", in the language of
        /// <paramref name="formatProvider"/> if that is a <see cref="CultureInfo"/>, otherwise of the current UI culture.
        /// "I" returns the interval, such as "[0 %RH, 30 %RH]", with the bounds formatted by <paramref name="formatProvider"/>.
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
                    var text = HumidityRanges.ResourceManager.GetString(this.resourceId, formatProvider as CultureInfo);
                    return text ?? this.resourceId;
            }
        }
    }
}