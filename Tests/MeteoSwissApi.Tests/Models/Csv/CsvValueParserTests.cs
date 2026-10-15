using MeteoSwissApi.Models.Csv;

namespace MeteoSwissApi.Tests.Models.Csv
{
    [Trait(Traits.Category, Traits.UnitTests)]
    public class CsvValueParserTests
    {
        [Theory]
        [InlineData("17.40", 17.4)]
        [InlineData("-3.40", -3.4)]
        [InlineData("1027.80", 1027.8)]
        [InlineData("0.00", 0)]
        public void TryParseDecimal_DecimalPointOnGermanCulture_ReturnsValue(string text, double expectedValue)
        {
            // Arrange
            using var cultureScope = new CultureScope("de-DE");

            // Act
            var result = CsvValueParser.TryParseDecimal(text, out var value);

            // Assert
            result.Should().BeTrue();
            value.Should().Be((decimal)expectedValue);
        }

        [Theory]
        [InlineData("-")]
        [InlineData("")]
        [InlineData(null)]
        public void TryParseDecimal_MissingValue_ReturnsFalse(string? text)
        {
            // Act
            var result = CsvValueParser.TryParseDecimal(text, out _);

            // Assert
            result.Should().BeFalse();
        }

        [Theory]
        [InlineData("47.479892", 47.479892)]
        [InlineData("8.904928", 8.904928)]
        public void TryParseDouble_DecimalPointOnGermanCulture_ReturnsValue(string text, double expectedValue)
        {
            // Arrange
            using var cultureScope = new CultureScope("de-DE");

            // Act
            var result = CsvValueParser.TryParseDouble(text, out var value);

            // Assert
            result.Should().BeTrue();
            value.Should().Be(expectedValue);
        }
    }
}
