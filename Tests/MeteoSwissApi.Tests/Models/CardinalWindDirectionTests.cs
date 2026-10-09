using System.Collections.ObjectModel;

namespace MeteoSwissApi.Tests.Models
{
    [Trait(Traits.Category, Traits.UnitTests)]
    public class CardinalWindDirectionTests
    {
        [Theory]
        [InlineData("en", "G", "North-northeast")]
        [InlineData("en", "A", "NNE")]
        [InlineData("en", "U", "NNE")]
        [InlineData("de", "G", "Nord-Nordost")]
        [InlineData("de", "A", "NNO")]
        [InlineData("de", "U", "NNE")]
        public void ToString_Format_ReturnsTextInUILanguage(string cultureName, string format, string expectedText)
        {
            // Arrange
            using var cultureScope = new CultureScope(cultureName);

            // Act
            var text = CardinalWindDirection.NNE.ToString(format);

            // Assert
            text.Should().Be(expectedText);
        }

        [Fact]
        public void ToString_NoFormat_ReturnsGeneralText()
        {
            // Arrange
            using var cultureScope = new CultureScope("en");

            // Act
            var text = CardinalWindDirection.E.ToString();

            // Assert
            text.Should().Be("East");
        }

        [Fact]
        public void ToString_CultureInfoProvider_ReturnsTextInThatLanguage()
        {
            // Arrange
            using var cultureScope = new CultureScope("en");

            // Act
            var text = CardinalWindDirection.E.ToString("G", new CultureInfo("de"));

            // Assert
            text.Should().Be("Ost");
        }

        [Fact]
        public void ToString_FormatProviderIsNoCulture_ReturnsText()
        {
            // Arrange
            using var cultureScope = new CultureScope("en");

            // Act
            var text = string.Format(NumberFormatInfo.InvariantInfo, "{0:A}", CardinalWindDirection.E);

            // Assert
            text.Should().Be("E");
        }

        [Fact]
        public void All_Always_IsReadOnly()
        {
            // Act
            var all = CardinalWindDirection.All;

            // Assert
            all.Should().BeOfType<ReadOnlyCollection<CardinalWindDirection>>();
            all.Should().HaveCount(16);
        }
    }
}
