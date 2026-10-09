namespace MeteoSwissApi.Extensions
{
    /// <summary>
    /// Provides conversion methods from wind direction to cardinal wind directions.
    /// </summary>
    /// <remarks>
    /// https://www.geographyrealm.com/cardinal-directions-ordinal-directions/
    /// </remarks>
    public static class AngleExtensions
    {
        private static readonly CardinalWindDirection[] CardinalDirections =
        {
            CardinalWindDirection.N, CardinalWindDirection.E, CardinalWindDirection.S, CardinalWindDirection.W,
        };

        private static readonly CardinalWindDirection[] IntercardinalDirections =
        {
            CardinalWindDirection.N, CardinalWindDirection.NE, CardinalWindDirection.E, CardinalWindDirection.SE,
            CardinalWindDirection.S, CardinalWindDirection.SW, CardinalWindDirection.W, CardinalWindDirection.NW,
        };

        private static readonly CardinalWindDirection[] SecondaryIntercardinalDirections =
        {
            CardinalWindDirection.N, CardinalWindDirection.NNE, CardinalWindDirection.NE, CardinalWindDirection.ENE,
            CardinalWindDirection.E, CardinalWindDirection.ESE, CardinalWindDirection.SE, CardinalWindDirection.SSE,
            CardinalWindDirection.S, CardinalWindDirection.SSW, CardinalWindDirection.SW, CardinalWindDirection.WSW,
            CardinalWindDirection.W, CardinalWindDirection.WNW, CardinalWindDirection.NW, CardinalWindDirection.NNW,
        };

        /// <summary>
        /// The four cardinal directions, or cardinal points, are the four main compass directions: 
        /// north, south, east, and west, commonly denoted by their initials N, S, E, and W respectively. 
        /// Relative to north, the directions east, south, and west are at 90 degree intervals in the clockwise direction.
        /// </summary>
        public static CardinalWindDirection ToCardinalWindDirection(this Angle windDirection)
        {
            return ToWindDirection(windDirection, CardinalDirections);
        }

        /// <summary>
        /// The intercardinal (intermediate, or, historically, ordinal) directions 
        /// are the four intermediate compass directions located halfway between each pair of cardinal directions.
        /// </summary>
        public static CardinalWindDirection ToIntercardinalWindDirection(this Angle windDirection)
        {
            return ToWindDirection(windDirection, IntercardinalDirections);
        }

        /// <summary>
        /// Directions midway between each cardinal and ordinal direction are referred to as secondary intercardinal directions.
        /// On a compass rose with ordinal, cardinal, and secondary intercardinal directions, there will be 16 points: 
        /// N, NNE, NE, ENE, E, ESE, SE, SSE, S, SSW, SW, WSW, W, WNW, NW, and NNW.
        /// </summary>
        /// <remarks>
        /// More info:
        /// http://snowfence.umn.edu/Components/winddirectionanddegrees.htm
        /// https://www.geographyrealm.com/cardinal-directions-ordinal-directions/
        /// </remarks>
        public static CardinalWindDirection ToSecondaryIntercardinalWindDirection(this Angle windDirection)
        {
            return ToWindDirection(windDirection, SecondaryIntercardinalDirections);
        }

        /// <summary>
        /// Returns the direction whose sector contains <paramref name="windDirection"/>. The angle is taken in degrees,
        /// whatever its unit, and normalized to the range 0° to 360°. Each sector is centered on its direction and
        /// includes its upper bound: with four directions, east covers everything above 45° up to and including 135°.
        /// </summary>
        private static CardinalWindDirection ToWindDirection(Angle windDirection, CardinalWindDirection[] directions)
        {
            var degrees = ((windDirection.Degrees % 360d) + 360d) % 360d;
            var sectorSize = 360d / directions.Length;
            var index = (int)Math.Ceiling((degrees / sectorSize) - 0.5d) % directions.Length;
            return directions[index];
        }
    }
}