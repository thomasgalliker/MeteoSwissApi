using System;
using System.Collections.Generic;
using System.Globalization;
using MeteoSwissApi.Resources.Strings;

namespace MeteoSwissApi.Models
{
    /// <summary>
    /// Cardinal wind direction splits a 360° circle into 16 slices of 22.5° each.
    /// </summary>
    public class CardinalWindDirection : IFormattable
    {
        public static readonly CardinalWindDirection NNE = new CardinalWindDirection(nameof(NNE));
        public static readonly CardinalWindDirection NE = new CardinalWindDirection(nameof(NE));
        public static readonly CardinalWindDirection ENE = new CardinalWindDirection(nameof(ENE));
        public static readonly CardinalWindDirection E = new CardinalWindDirection(nameof(E));
        public static readonly CardinalWindDirection ESE = new CardinalWindDirection(nameof(ESE));
        public static readonly CardinalWindDirection SE = new CardinalWindDirection(nameof(SE));
        public static readonly CardinalWindDirection SSE = new CardinalWindDirection(nameof(SSE));
        public static readonly CardinalWindDirection S = new CardinalWindDirection(nameof(S));
        public static readonly CardinalWindDirection SSW = new CardinalWindDirection(nameof(SSW));
        public static readonly CardinalWindDirection SW = new CardinalWindDirection(nameof(SW));
        public static readonly CardinalWindDirection WSW = new CardinalWindDirection(nameof(WSW));
        public static readonly CardinalWindDirection W = new CardinalWindDirection(nameof(W));
        public static readonly CardinalWindDirection WNW = new CardinalWindDirection(nameof(WNW));
        public static readonly CardinalWindDirection NW = new CardinalWindDirection(nameof(NW));
        public static readonly CardinalWindDirection NNW = new CardinalWindDirection(nameof(NNW));
        public static readonly CardinalWindDirection N = new CardinalWindDirection(nameof(N));

        public static readonly IEnumerable<CardinalWindDirection> All = new List<CardinalWindDirection>
        {
            NNE, NE, ENE, E, ESE, SE, SSE, S, SSW, SW, WSW, W, WNW, NW, NNW, N
        };

        private readonly string resourceId;

        private CardinalWindDirection(string resourceId)
        {
            this.resourceId = resourceId;
        }

        /// <summary>
        /// Converts the value of the current CardinalWindDirection object to its equivalent string representation using the formatting conventions of the current culture.
        /// </summary>
        /// <returns>A string representation of value of the current CardinalWindDirection object.</returns>
        public override string ToString()
        {
            return this.ToString(null, null);
        }

        /// <inheritdoc cref="ToString(string, IFormatProvider)"/>
        /// <summary>
        /// YYYConverts the value of the current CardinalWindDirection object to its equivalent string representation using the specified format and the formatting conventions of the current culture.
        /// </summary>
        /// <param name="format">A standard format string.</param>
        /// <returns>A string representation of value of the current CardinalWindDirection object as specified by format.</returns>
        public string ToString(string format)
        {
            return this.ToString(format, null);
        }

        /// <summary>
        /// Converts the value of the current CardinalWindDirection object to its equivalent string representation using the formatting conventions of the current culture.
        /// </summary>
        /// <param name="format">A standard format string.</param>
        /// <remarks>
        /// The valid format strings are as follows:
        /// <list type="bullet">
        ///     <item>
        ///         <term>"G" or "g".</term>
        ///         <description>The general string translation for <see cref="CardinalWindDirection" />, such as "North-Northwest".</description>
        ///     </item>
        ///     <item>
        ///         <term>"A" or "a".</term>
        ///         <description>The abbreviated string translation for <see cref="CardinalWindDirection" />, such as "NNW".</description>
        ///     </item>
        ///     <item>
        ///         <term>"U" or "u".</term>
        ///         <description>The enum name of <see cref="CardinalWindDirection" />, such as "NNW".</description>
        ///     </item>
        /// </list>
        /// </remarks>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>A string representation of value of the current CardinalWindDirection object as specified by format.</returns>
        public string ToString(string format, IFormatProvider provider)
        {
            var culture = provider as CultureInfo ?? CultureInfo.CurrentCulture;

            if (string.IsNullOrWhiteSpace(format))
            {
                format = "g";
            }

            var formatSpecifier = format![0];


            switch (formatSpecifier)
            {
                case 'a':
                case 'A':
                    return CardinalWindDirectionsAcronyms.ResourceManager.GetString(this.resourceId, culture);
                case 'u':
                case 'U':
                    return this.resourceId;
                case 'g':
                case 'G':
                default:
                    return CardinalWindDirections.ResourceManager.GetString(this.resourceId, culture);
            }
        }
    }
}