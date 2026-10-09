using System.Collections.ObjectModel;

namespace MeteoSwissApi.Tests.Models
{
    [Trait(Traits.Category, Traits.UnitTests)]
    public class PressureRangeTests
    {
        [Theory]
        [ClassData(typeof(PressureRangeTestData))]
        public void FromValue_PressureInRange_ReturnsRange(Pressure pressure, PressureRange expectedPressureRange)
        {
            // Act
            var pressureRange = PressureRange.FromValue(pressure);

            // Assert
            pressureRange.Should().Be(expectedPressureRange);
        }

        public class PressureRangeTestData : TheoryData<Pressure, PressureRange>
        {
            public PressureRangeTestData()
            {
                this.Add(Pressure.FromHectopascals(0), PressureRange.VeryLow);
                this.Add(Pressure.FromHectopascals(998), PressureRange.VeryLow);
                this.Add(Pressure.FromHectopascals(999), PressureRange.Low);
                this.Add(Pressure.FromHectopascals(1007), PressureRange.Low);
                this.Add(Pressure.FromHectopascals(1008), PressureRange.Average);
                this.Add(Pressure.FromHectopascals(1018), PressureRange.Average);
                this.Add(Pressure.FromHectopascals(1019), PressureRange.High);
                this.Add(Pressure.FromHectopascals(1027), PressureRange.High);
                this.Add(Pressure.FromHectopascals(1028), PressureRange.VeryHigh);
                this.Add(Pressure.FromHectopascals(1044), PressureRange.VeryHigh);
            }
        }

        [Fact]
        public void FromValue_NegativePressure_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            var pressure = Pressure.FromHectopascals(-1);

            // Act
            Action action = () => PressureRange.FromValue(pressure);

            // Assert
            action.Should().Throw<ArgumentOutOfRangeException>().WithMessage("*must not be negative*");
        }

        [Theory]
        [InlineData("en", "Very low")]
        [InlineData("de", "Sehr tief")]
        public void ToString_UICulture_ReturnsNameInUILanguage(string cultureName, string expectedText)
        {
            // Arrange
            using var cultureScope = new CultureScope(cultureName);

            // Act
            var text = PressureRange.VeryLow.ToString();

            // Assert
            text.Should().Be(expectedText);
        }

        [Fact]
        public void ToString_FormatProviderIsNoCulture_ReturnsName()
        {
            // Arrange
            using var cultureScope = new CultureScope("en");

            // Act
            var text = string.Format(NumberFormatInfo.InvariantInfo, "{0}", PressureRange.VeryLow);

            // Assert
            text.Should().Be("Very low");
        }

        [Fact]
        public void ToString_FormatI_ReturnsInterval()
        {
            // Arrange
            using var cultureScope = new CultureScope("en");

            // Act
            var text = PressureRange.Average.ToString("I");

            // Assert
            // UnitsNet formats the bounds with the culture's digit grouping, for example "1,008 hPa".
            text.Should().MatchRegex(@"^\[1[,.]?008 hPa, 1[,.]?018 hPa\]$");
        }

        [Fact]
        public void All_Always_IsReadOnly()
        {
            // Act
            var all = PressureRange.All;

            // Assert
            all.Should().BeOfType<ReadOnlyCollection<PressureRange>>();
            all.Should().Equal(PressureRange.VeryLow, PressureRange.Low, PressureRange.Average, PressureRange.High, PressureRange.VeryHigh);
        }
    }
}