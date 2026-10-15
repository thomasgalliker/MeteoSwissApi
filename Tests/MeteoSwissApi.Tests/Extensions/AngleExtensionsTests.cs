namespace MeteoSwissApi.Tests.Extensions
{
    [Trait(Traits.Category, Traits.UnitTests)]
    public class AngleExtensionsTests
    {
        [Theory]
        [ClassData(typeof(CardinalWindDirectionTestData))]
        public void ToCardinalWindDirection_Angle_ReturnsDirection(Angle windDirection, CardinalWindDirection expectedCardinalWindDirection)
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
        public void ToIntercardinalWindDirection_Angle_ReturnsDirection(Angle windDirection, CardinalWindDirection expectedCardinalWindDirection)
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
        public void ToSecondaryIntercardinalWindDirection_Angle_ReturnsDirection(Angle windDirection, CardinalWindDirection expectedCardinalWindDirection)
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

        [Fact]
        public void ToCardinalWindDirection_AngleInRadians_UsesDegrees()
        {
            // Arrange
            var windDirection = Angle.FromRadians(Math.PI);

            // Act
            var cardinalWindDirection = windDirection.ToCardinalWindDirection();

            // Assert
            cardinalWindDirection.Should().Be(CardinalWindDirection.S);
        }

        [Theory]
        [ClassData(typeof(AngleOutsideFullCircleTestData))]
        public void ToCardinalWindDirection_AngleOutsideFullCircle_NormalizesAngle(Angle windDirection, CardinalWindDirection expectedCardinalWindDirection)
        {
            // Act
            var cardinalWindDirection = windDirection.ToCardinalWindDirection();

            // Assert
            cardinalWindDirection.Should().Be(expectedCardinalWindDirection);
        }

        public class AngleOutsideFullCircleTestData : TheoryData<Angle, CardinalWindDirection>
        {
            public AngleOutsideFullCircleTestData()
            {
                this.Add(Angle.FromDegrees(360), CardinalWindDirection.N);
                this.Add(Angle.FromDegrees(450), CardinalWindDirection.E);
                this.Add(Angle.FromDegrees(-90), CardinalWindDirection.W);
                this.Add(Angle.FromDegrees(-180), CardinalWindDirection.S);
            }
        }

        [Theory]
        [ClassData(typeof(CardinalSectorBoundaryTestData))]
        public void ToCardinalWindDirection_SectorBoundary_UpperBoundBelongsToSector(Angle windDirection, CardinalWindDirection expectedCardinalWindDirection)
        {
            // Act
            var cardinalWindDirection = windDirection.ToCardinalWindDirection();

            // Assert
            cardinalWindDirection.Should().Be(expectedCardinalWindDirection);
        }

        public class CardinalSectorBoundaryTestData : TheoryData<Angle, CardinalWindDirection>
        {
            public CardinalSectorBoundaryTestData()
            {
                this.Add(Angle.FromDegrees(0), CardinalWindDirection.N);
                this.Add(Angle.FromDegrees(45), CardinalWindDirection.N);
                this.Add(Angle.FromDegrees(45.1), CardinalWindDirection.E);
                this.Add(Angle.FromDegrees(135), CardinalWindDirection.E);
                this.Add(Angle.FromDegrees(315), CardinalWindDirection.W);
                this.Add(Angle.FromDegrees(315.1), CardinalWindDirection.N);
            }
        }

        [Theory]
        [ClassData(typeof(IntercardinalSectorBoundaryTestData))]
        public void ToIntercardinalWindDirection_SectorBoundary_UpperBoundBelongsToSector(Angle windDirection, CardinalWindDirection expectedCardinalWindDirection)
        {
            // Act
            var intercardinalWindDirection = windDirection.ToIntercardinalWindDirection();

            // Assert
            intercardinalWindDirection.Should().Be(expectedCardinalWindDirection);
        }

        public class IntercardinalSectorBoundaryTestData : TheoryData<Angle, CardinalWindDirection>
        {
            public IntercardinalSectorBoundaryTestData()
            {
                this.Add(Angle.FromDegrees(22.5), CardinalWindDirection.N);
                this.Add(Angle.FromDegrees(22.6), CardinalWindDirection.NE);
                this.Add(Angle.FromDegrees(67.5), CardinalWindDirection.NE);
                this.Add(Angle.FromDegrees(337.5), CardinalWindDirection.NW);
                this.Add(Angle.FromDegrees(337.6), CardinalWindDirection.N);
            }
        }

        [Theory]
        [ClassData(typeof(SecondaryIntercardinalSectorBoundaryTestData))]
        public void ToSecondaryIntercardinalWindDirection_SectorBoundary_UpperBoundBelongsToSector(Angle windDirection, CardinalWindDirection expectedCardinalWindDirection)
        {
            // Act
            var secondaryIntercardinalWindDirection = windDirection.ToSecondaryIntercardinalWindDirection();

            // Assert
            secondaryIntercardinalWindDirection.Should().Be(expectedCardinalWindDirection);
        }

        public class SecondaryIntercardinalSectorBoundaryTestData : TheoryData<Angle, CardinalWindDirection>
        {
            public SecondaryIntercardinalSectorBoundaryTestData()
            {
                this.Add(Angle.FromDegrees(11.25), CardinalWindDirection.N);
                this.Add(Angle.FromDegrees(11.26), CardinalWindDirection.NNE);
                this.Add(Angle.FromDegrees(33.75), CardinalWindDirection.NNE);
                this.Add(Angle.FromDegrees(281.25), CardinalWindDirection.W);
                this.Add(Angle.FromDegrees(281.26), CardinalWindDirection.WNW);
                this.Add(Angle.FromDegrees(348.75), CardinalWindDirection.NNW);
                this.Add(Angle.FromDegrees(348.76), CardinalWindDirection.N);
            }
        }
    }
}