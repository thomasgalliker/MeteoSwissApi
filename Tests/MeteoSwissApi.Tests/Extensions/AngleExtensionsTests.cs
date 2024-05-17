using FluentAssertions;
using MeteoSwissApi.Extensions;
using MeteoSwissApi.Models;
using UnitsNet;
using UnitsNet.Units;
using Xunit;

namespace MeteoSwissApi.Tests.Extensions
{
    public class AngleExtensionsTests
    {
        [Theory]
        [ClassData(typeof(CardinalWindDirectionTestData))]
        public void ShouldGetCardinalWindDirection(Angle windDirection, CardinalWindDirection expectedCardinalWindDirection)
        {
            // Act
            var cardinalWindDirection = windDirection.ToCardinalWindDirection();

            // Assert
            cardinalWindDirection.Should().Be(expectedCardinalWindDirection);
        }

        public class CardinalWindDirectionTestData : TheoryData<Angle, CardinalWindDirection>
        {
            public CardinalWindDirectionTestData()
            {
                this.Add(new Angle(120, AngleUnit.Degree), CardinalWindDirection.E);
                this.Add(new Angle(146, AngleUnit.Degree), CardinalWindDirection.S);
                this.Add(new Angle(147, AngleUnit.Degree), CardinalWindDirection.S);
            }
        }

        [Theory]
        [ClassData(typeof(IntercardinalWindDirectionTestData))]
        public void ShouldGetIntercardinalWindDirection(Angle windDirection, CardinalWindDirection expectedCardinalWindDirection)
        {
            // Act
            var intercardinalWindDirection = windDirection.ToIntercardinalWindDirection();

            // Assert
            intercardinalWindDirection.Should().Be(expectedCardinalWindDirection);
        }

        public class IntercardinalWindDirectionTestData : TheoryData<Angle, CardinalWindDirection>
        {
            public IntercardinalWindDirectionTestData()
            {
                this.Add(new Angle(120, AngleUnit.Degree), CardinalWindDirection.SE);
                this.Add(new Angle(146, AngleUnit.Degree), CardinalWindDirection.SE);
                this.Add(new Angle(147, AngleUnit.Degree), CardinalWindDirection.SE);
            }
        }

        [Theory]
        [ClassData(typeof(SecondaryIntercardinalWindDirectionTestData))]
        public void ShouldGetSecondaryIntercardinalWindDirection(Angle windDirection, CardinalWindDirection expectedCardinalWindDirection)
        {
            // Act
            var SecondaryIntercardinalWindDirection = windDirection.ToSecondaryIntercardinalWindDirection();

            // Assert
            SecondaryIntercardinalWindDirection.Should().Be(expectedCardinalWindDirection);
        }

        public class SecondaryIntercardinalWindDirectionTestData : TheoryData<Angle, CardinalWindDirection>
        {
            public SecondaryIntercardinalWindDirectionTestData()
            {
                this.Add(new Angle(120, AngleUnit.Degree), CardinalWindDirection.ESE);
                this.Add(new Angle(146, AngleUnit.Degree), CardinalWindDirection.SE);
                this.Add(new Angle(147, AngleUnit.Degree), CardinalWindDirection.SSE);
            }
        }
    }
}