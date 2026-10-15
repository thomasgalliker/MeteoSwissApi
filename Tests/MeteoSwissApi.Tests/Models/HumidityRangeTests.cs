using System.Collections.ObjectModel;

namespace MeteoSwissApi.Tests.Models
{
    [Trait(Traits.Category, Traits.UnitTests)]
    public class HumidityRangeTests
    {
        [Theory]
        [ClassData(typeof(HumidityRangeTestData))]
        public void FromValue_HumidityInRange_ReturnsRange(RelativeHumidity humidity, HumidityRange expectedHumidityRange)
        {
            // Act
            var humidityRange = HumidityRange.FromValue(humidity);

            // Assert
            humidityRange.Should().Be(expectedHumidityRange);
        }

        public class HumidityRangeTestData : TheoryData<RelativeHumidity, HumidityRange>
        {
            public HumidityRangeTestData()
            {
                this.Add(RelativeHumidity.FromPercent(0), HumidityRange.VeryDry);
                this.Add(RelativeHumidity.FromPercent(30), HumidityRange.VeryDry);
                this.Add(RelativeHumidity.FromPercent(31), HumidityRange.Dry);
                this.Add(RelativeHumidity.FromPercent(39), HumidityRange.Dry);
                this.Add(RelativeHumidity.FromPercent(40), HumidityRange.Average);
                this.Add(RelativeHumidity.FromPercent(70), HumidityRange.Average);
                this.Add(RelativeHumidity.FromPercent(71), HumidityRange.Moist);
                this.Add(RelativeHumidity.FromPercent(79), HumidityRange.Moist);
                this.Add(RelativeHumidity.FromPercent(80), HumidityRange.VeryMoist);
                this.Add(RelativeHumidity.FromPercent(100), HumidityRange.VeryMoist);
            }
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-120)]
        public void FromValue_NegativeHumidity_ThrowsArgumentOutOfRangeException(double percent)
        {
            // Arrange
            var humidity = RelativeHumidity.FromPercent(percent);

            // Act
            Action action = () => HumidityRange.FromValue(humidity);

            // Assert
            action.Should().Throw<ArgumentOutOfRangeException>().WithMessage("*must not be negative*");
        }

        [Theory]
        [InlineData("en", "Very dry")]
        [InlineData("de", "Sehr trocken")]
        public void ToString_UICulture_ReturnsNameInUILanguage(string cultureName, string expectedText)
        {
            // Arrange
            using var cultureScope = new CultureScope(cultureName);

            // Act
            var text = HumidityRange.VeryDry.ToString();

            // Assert
            text.Should().Be(expectedText);
        }

        [Fact]
        public void ToString_FormatProviderIsNoCulture_ReturnsName()
        {
            // Arrange
            using var cultureScope = new CultureScope("en");

            // Act
            var text = string.Format(NumberFormatInfo.InvariantInfo, "{0}", HumidityRange.VeryDry);

            // Assert
            text.Should().Be("Very dry");
        }

        [Fact]
        public void ToString_FormatI_ReturnsInterval()
        {
            // Arrange
            using var cultureScope = new CultureScope("en");

            // Act
            var text = HumidityRange.Dry.ToString("I");

            // Assert
            text.Should().StartWith("(").And.EndWith(")").And.Contain("30").And.Contain("40");
        }

        [Fact]
        public void All_Always_IsReadOnly()
        {
            // Act
            var all = HumidityRange.All;

            // Assert
            all.Should().BeOfType<ReadOnlyCollection<HumidityRange>>();
            all.Should().Equal(HumidityRange.VeryDry, HumidityRange.Dry, HumidityRange.Average, HumidityRange.Moist, HumidityRange.VeryMoist);
        }
        [Fact]
        public void FromValue_Above100Percent_ReturnsVeryMoist()
        {
            // Act
            var humidityRange = HumidityRange.FromValue(RelativeHumidity.FromPercent(100.5));

            // Assert
            humidityRange.Should().Be(HumidityRange.VeryMoist);
        }

        [Fact]
        public void ToString_FormatIWithProvider_ReturnsInterval()
        {
            // Act
            var text = HumidityRange.VeryDry.ToString("I", CultureInfo.InvariantCulture);

            // Assert
            text.Should().StartWith("[0 ").And.EndWith("]").And.Contain("30 ");
        }
    }
}
