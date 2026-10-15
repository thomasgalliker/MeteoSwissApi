namespace MeteoSwissApi.Models
{
    [JsonConverter(typeof(SlfStationTypeJsonConverter))]
    [DebuggerDisplay("{this.value}")]
    public readonly struct SlfStationType : IEquatable<SlfStationType>, IFormattable
    {
        public const string SnowFlat = "SNOW_FLAT";

        public const string SnowSlope = "SNOW_SLOPE";

        public const string Wind = "WIND";

        public const string Special = "SPECIAL";

        public const string FlowCapt = "FLOWCAPT";

        public const string Pluvio = "PLUVIO";

        /// <summary>
        /// The known station types.
        /// </summary>
        public static readonly IReadOnlyList<SlfStationType> All = Array.AsReadOnly(new[]
        {
            new SlfStationType(SnowFlat),
            new SlfStationType(SnowSlope),
            new SlfStationType(Wind),
            new SlfStationType(Special),
            new SlfStationType(FlowCapt),
            new SlfStationType(Pluvio),
        });

        private readonly string? value;

        public SlfStationType(string value)
        {
            this.value = value;
        }

        public string? Value => this.value;

        public static implicit operator string?(SlfStationType u) => u.value;

        public static explicit operator SlfStationType(string value) => new SlfStationType(value);

        public bool Equals(SlfStationType other)
        {
            return string.Equals(this.value, other.value, StringComparison.Ordinal);
        }

        public override bool Equals(object? obj)
        {
            return obj is SlfStationType other && this.Equals(other);
        }

        public override int GetHashCode()
        {
            return this.value is null ? 0 : StringComparer.Ordinal.GetHashCode(this.value);
        }

        public static bool operator ==(SlfStationType left, SlfStationType right) => left.Equals(right);

        public static bool operator !=(SlfStationType left, SlfStationType right) => !left.Equals(right);

        public override string ToString()
        {
            return this.ToString(null, null);
        }

        public string ToString(string? format)
        {
            return this.ToString(format, null);
        }

        /// <summary>
        /// Formats the station type: "G" (default) returns its text, such as "Wind station", in the language of
        /// <paramref name="formatProvider"/> if that is a <see cref="CultureInfo"/>, otherwise of the current UI culture.
        /// Any other format returns the value, such as "WIND".
        /// </summary>
        public string ToString(string? format, IFormatProvider? formatProvider)
        {
            if (string.IsNullOrEmpty(format))
            {
                format = "G";
            }

            switch (format)
            {
                case "G":
                    if (this.value == null)
                    {
                        return string.Empty;
                    }

                    var translation = SlfStationTypes.ResourceManager.GetString(this.value, formatProvider as CultureInfo);
                    return translation ?? this.value;
                default:
                    return this.value ?? string.Empty;
            }
        }
    }
}
