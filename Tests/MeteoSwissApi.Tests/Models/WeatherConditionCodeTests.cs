using System.Collections.ObjectModel;

namespace MeteoSwissApi.Tests.Models
{
    [Trait(Traits.Category, Traits.UnitTests)]
    public class WeatherConditionCodeTests
    {
        [Theory]
        [InlineData(-100)]
        [InlineData(0)]
        [InlineData(43)]
        public void FromValue_UnknownValue_ThrowsArgumentOutOfRangeException(int value)
        {
            // Act
            Action action = () => WeatherConditionCode.FromValue(value);

            // Assert
            action.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(42)]
        [InlineData(51)]
        [InlineData(142)]
        public void FromValue_KnownValue_ReturnsCode(int value)
        {
            // Act
            var weatherConditionCode = WeatherConditionCode.FromValue(value);

            // Assert
            weatherConditionCode.Value.Should().Be(value);
        }

        [Fact]
        public void TryGetFromValue_UnknownValue_ReturnsFalseAndUnknown()
        {
            // Act
            var success = WeatherConditionCode.TryGetFromValue(999, out var weatherConditionCode);

            // Assert
            success.Should().BeFalse();
            weatherConditionCode.Should().Be(WeatherConditionCode.Unknown);
        }

        [Fact]
        public async Task TryGetFromValue_EveryHighContrastIcon_ReturnsCode()
        {
            // Arrange
            var range = Enumerable.Range(1, 200).ToArray();

            var weatherIconMapping = new HighContrastWeatherIconMapping();
            var embeddedIcons = await TestHelper.TryGetIconsAsync(range, iconId => weatherIconMapping.GetIconAsync(iconId));

            static (int Id, WeatherConditionCode WeatherConditionCode, bool Success) TryCreateWeatherConditionCode((int IconId, Stream Stream) i)
            {
                var success = WeatherConditionCode.TryGetFromValue(i.IconId, out var weatherConditionCode);
                return (i.IconId, weatherConditionCode, success);
            }

            // Act
            var weatherConditionCodes = embeddedIcons.Select(TryCreateWeatherConditionCode).ToArray();

            // Assert
            embeddedIcons.Should().NotBeEmpty();

            var weatherConditionCodesUnsuccessful = weatherConditionCodes
                .Where(c => !c.Success)
                .Select(c => c.Id)
                .ToArray();

            weatherConditionCodesUnsuccessful.Length.Should().Be(
                expected: 0,
                because: $"{nameof(WeatherConditionCode)}.{nameof(WeatherConditionCode.FromValue)} failed for icons with Id: {Environment.NewLine}" +
                $"[{string.Join(", ", weatherConditionCodesUnsuccessful)}]");
        }

        [Theory]
        [InlineData("en", 1, "Sunny")]
        [InlineData("de", 1, "Sonnig")]
        [InlineData("de", 3, "Teilweise sonnig")]
        public void ToString_UICulture_ReturnsTextInUILanguage(string cultureName, int value, string expectedText)
        {
            // Arrange
            using var cultureScope = new CultureScope(cultureName);
            var weatherConditionCode = WeatherConditionCode.FromValue(value);

            // Act
            var text = weatherConditionCode.ToString();

            // Assert
            text.Should().Be(expectedText);
        }

        [Fact]
        public void ToString_EveryCode_HasGermanText()
        {
            // Arrange
            var cultureInfo = new CultureInfo("de");

            // Act
            var texts = WeatherConditionCode.All.Select(c => (Code: c.Value, Text: c.ToString("G", cultureInfo))).ToArray();

            // Assert
            // The fallback is the code itself, so a translated code never prints as its number.
            texts.Should().OnlyContain(t => t.Text != t.Code.ToString(CultureInfo.InvariantCulture));
        }

        [Fact]
        public void ToString_FormatProviderIsNoCulture_ReturnsText()
        {
            // Arrange
            using var cultureScope = new CultureScope("en");

            // Act
            var text = string.Format(NumberFormatInfo.InvariantInfo, "{0}", WeatherConditionCode.Code1);

            // Assert
            text.Should().Be("Sunny");
        }

        [Fact]
        public void ToString_OtherFormat_ReturnsValue()
        {
            // Act
            var text = WeatherConditionCode.Code142.ToString("V");

            // Assert
            text.Should().Be("142");
        }

        [Fact]
        public void CompareTo_BoxedWeatherConditionCode_ComparesValues()
        {
            // Arrange
            IComparable weatherConditionCode = WeatherConditionCode.Code1;

            // Act
            var result = weatherConditionCode.CompareTo(WeatherConditionCode.Code2);

            // Assert
            result.Should().BeNegative();
        }

        [Fact]
        public void CompareTo_OtherType_ThrowsArgumentException()
        {
            // Arrange
            IComparable weatherConditionCode = WeatherConditionCode.Code1;

            // Act
            Action action = () => weatherConditionCode.CompareTo("Code1");

            // Assert
            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void ExplicitConversion_KnownValue_ReturnsCode()
        {
            // Act
            var weatherConditionCode = (WeatherConditionCode)142;

            // Assert
            weatherConditionCode.Should().Be(WeatherConditionCode.Code142);
        }

        [Fact]
        public void ExplicitConversion_UnknownValue_ThrowsArgumentOutOfRangeException()
        {
            // Act
            Action action = () => _ = (WeatherConditionCode)43;

            // Assert
            action.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void Equals_SameValue_ReturnsTrueWithSameHashCode()
        {
            // Arrange
            var weatherConditionCode = WeatherConditionCode.FromValue(1);
            object boxed = WeatherConditionCode.Code1;

            // Act
            var equalsObject = weatherConditionCode.Equals(boxed);
            var equalsOperator = weatherConditionCode == WeatherConditionCode.Code1;
            var differsOperator = weatherConditionCode != WeatherConditionCode.Code2;

            // Assert
            equalsObject.Should().BeTrue();
            equalsOperator.Should().BeTrue();
            differsOperator.Should().BeTrue();
            weatherConditionCode.GetHashCode().Should().Be(WeatherConditionCode.Code1.GetHashCode());
        }

        [Fact]
        public void ComparisonOperators_Always_CompareValues()
        {
            // Act
            var less = WeatherConditionCode.Code1 < WeatherConditionCode.Code2;
            var greaterOrEqual = WeatherConditionCode.Code142 >= WeatherConditionCode.Code101;

            // Assert
            less.Should().BeTrue();
            greaterOrEqual.Should().BeTrue();
        }

        [Fact]
        public void Unknown_Always_IsTheDefaultValue()
        {
            // Act
            var unknown = WeatherConditionCode.Unknown;

            // Assert
            unknown.Should().Be(default(WeatherConditionCode));
            unknown.Value.Should().Be(0);
        }

        [Fact]
        public void All_Always_IsReadOnly()
        {
            // Act
            var all = WeatherConditionCode.All;

            // Assert
            all.Should().BeOfType<ReadOnlyCollection<WeatherConditionCode>>();
            all.Should().HaveCount(112);
            all.Should().BeInAscendingOrder(c => c.Value);
        }
    }
}