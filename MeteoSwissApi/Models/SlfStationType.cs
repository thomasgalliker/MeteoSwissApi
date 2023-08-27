using System.Diagnostics;

namespace MeteoSwissApi.Models
{
    [DebuggerDisplay("{this.name}")]
    public struct SlfStationType
    {
        public const string SnowFlat = "SNOW_FLAT";
        public const string SnowSlope = "SNOW_SLOPE";
        public const string Wind = "WIND";
        public const string Special = "SPECIAL";
        public const string FlowCapt = "FLOWCAPT";
        public const string Pluvio = "PLUVIO";

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
            return this.value;
        }
    }
}