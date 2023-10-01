using MeteoSwissApi.Models;
using UnitsNet;

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
        /// <summary>
        /// The four cardinal directions, or cardinal points, are the four main compass directions: 
        /// north, south, east, and west, commonly denoted by their initials N, S, E, and W respectively. 
        /// Relative to north, the directions east, south, and west are at 90 degree intervals in the clockwise direction.
        /// </summary>
        public static CardinalWindDirection ToCardinalWindDirection(this Angle windDirection)
        {
            var windDegrees = windDirection.Value;

            if (windDegrees is > 45 and <= 135)
            {
                return CardinalWindDirection.E;
            }

            if (windDegrees is > 135 and <= 225)
            {
                return CardinalWindDirection.S;
            }

            if (windDegrees is > 225 and <= 315)
            {
                return CardinalWindDirection.W;
            }

            return CardinalWindDirection.N;
        }

        /// <summary>
        /// The intercardinal (intermediate, or, historically, ordinal) directions 
        /// are the four intermediate compass directions located halfway between each pair of cardinal directions.
        /// </summary>
        public static CardinalWindDirection ToIntercardinalWindDirection(this Angle windDirection)
        {
            var windDegrees = windDirection.Value;

            if (windDegrees is > 22.5 and <= 67.5)
            {
                return CardinalWindDirection.NE;
            }

            if (windDegrees is > 67.5 and <= 112.5)
            {
                return CardinalWindDirection.E;
            }

            if (windDegrees is > 112.5 and <= 157.5)
            {
                return CardinalWindDirection.SE;
            }


            if (windDegrees is > 157.5 and <= 202.5)
            {
                return CardinalWindDirection.S;
            }


            if (windDegrees is > 202.5 and <= 247.5)
            {
                return CardinalWindDirection.SW;
            }


            if (windDegrees is > 247.5 and <= 292.5)
            {
                return CardinalWindDirection.W;
            }

            if (windDegrees is > 292.5 and <= 337.5)
            {
                return CardinalWindDirection.NW;
            }

            return CardinalWindDirection.N;
        }

        /// <summary>
        /// Directions midway between each cardinal and ordinal direction are referred to as secondary intercardinal directions.
        /// On a compass rose with ordinal, cardinal, and secondary intercardinal directions, there will be 16 points: 
        /// N, NNE, NE, ENE, E, ESE, SE, SSE, S, SSW, SW, WSW, W, NWN, NW, and NNW.
        /// </summary>
        /// <remarks>
        /// More info:
        /// http://snowfence.umn.edu/Components/winddirectionanddegrees.htm
        /// https://www.geographyrealm.com/cardinal-directions-ordinal-directions/
        /// </remarks>
        public static CardinalWindDirection ToSecondaryIntercardinalWindDirection(this Angle windDirection)
        {
            var windDegrees = windDirection.Value;

            if (windDegrees is > 11.25 and <= 33.75)
            {
                return CardinalWindDirection.NNE;
            }

            if (windDegrees is > 33.75 and <= 56.25)
            {
                return CardinalWindDirection.NE;
            }

            if (windDegrees is > 56.25 and <= 78.75)
            {
                return CardinalWindDirection.ENE;
            }

            if (windDegrees is > 78.75 and <= 101.25)
            {
                return CardinalWindDirection.E;
            }

            if (windDegrees is > 101.25 and <= 123.75)
            {
                return CardinalWindDirection.ESE;
            }

            if (windDegrees is > 123.75 and <= 146.25)
            {
                return CardinalWindDirection.SE;
            }

            if (windDegrees is > 146.25 and <= 168.75)
            {
                return CardinalWindDirection.SSE;
            }

            if (windDegrees is > 168.75 and <= 191.25)
            {
                return CardinalWindDirection.S;
            }

            if (windDegrees is > 191.25 and <= 213.75)
            {
                return CardinalWindDirection.SSW;
            }

            if (windDegrees is > 213.75 and <= 236.25)
            {
                return CardinalWindDirection.SW;
            }

            if (windDegrees is > 236.25 and <= 258.75)
            {
                return CardinalWindDirection.WSW;
            }

            if (windDegrees is > 258.75 and <= 281.25)
            {
                return CardinalWindDirection.W;
            }

            if (windDegrees is > 281.25 and <= 303.75)
            {
                return CardinalWindDirection.WNW;
            }

            if (windDegrees is > 303.75 and <= 326.25)
            {
                return CardinalWindDirection.NW;
            }

            if (windDegrees is > 326.25 and <= 348.75)
            {
                return CardinalWindDirection.NNW;
            }

            return CardinalWindDirection.N;


        }
    }
}