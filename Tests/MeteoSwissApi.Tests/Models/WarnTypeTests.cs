using System.Collections.ObjectModel;

namespace MeteoSwissApi.Tests.Models
{
    [Trait(Traits.Category, Traits.UnitTests)]
    public class WarnTypeTests
    {
        [Theory]
        [ClassData(typeof(WarnTypeTestData))]
        public void FromValue_KnownValue_ReturnsWarnType(int warnTypeValue, WarnType expectedWarnType)
        {
            // Act
            var warnType = WarnType.FromValue(warnTypeValue);

            // Assert
            warnType.Should().Be(expectedWarnType);
        }

        public class WarnTypeTestData : TheoryData<int, WarnType>
        {
            public WarnTypeTestData()
            {
                this.Add(0, WarnType.Wind);
                this.Add(1, WarnType.Thunderstorms);
                this.Add(2, WarnType.Rain);
                this.Add(12, WarnType.Drought);
            }
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(13)]
        public void FromValue_UnknownValue_ThrowsArgumentOutOfRangeException(int warnTypeValue)
        {
            // Act
            Action action = () => WarnType.FromValue(warnTypeValue);

            // Assert
            action.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Theory]
        [InlineData("en", 0, "Wind")]
        [InlineData("en", 10, "Forest fire")]
        [InlineData("en", 12, "Drought")]
        [InlineData("de", 8, "Lawinen")]
        [InlineData("de", 12, "Trockenheit")]
        public void ToString_UICulture_ReturnsTextInUILanguage(string cultureName, int warnTypeValue, string expectedText)
        {
            // Arrange
            using var cultureScope = new CultureScope(cultureName);
            var warnType = WarnType.FromValue(warnTypeValue);

            // Act
            var text = warnType.ToString();

            // Assert
            text.Should().Be(expectedText);
        }

        [Fact]
        public void ToString_UICultureDiffersFromCulture_UsesUICulture()
        {
            // Arrange
            using var cultureScope = new CultureScope("de-CH", "en");

            // Act
            var text = WarnType.Avalanches.ToString();

            // Assert
            text.Should().Be("Avalanches");
        }

        [Fact]
        public void ToString_FormatProviderIsNoCulture_ReturnsText()
        {
            // Arrange
            using var cultureScope = new CultureScope("en");

            // Act
            var text = string.Format(NumberFormatInfo.InvariantInfo, "{0}", WarnType.Avalanches);

            // Assert
            text.Should().Be("Avalanches");
        }

        [Fact]
        public void ToString_OtherFormat_ReturnsName()
        {
            // Act
            var text = WarnType.ForestFire.ToString("U");

            // Assert
            text.Should().Be("ForestFire");
        }

        [Fact]
        public void ToString_DefaultWarnType_ReturnsValue()
        {
            // Arrange
            var warnType = default(WarnType);

            // Act
            var stringOutput = warnType.ToString();

            // Assert
            stringOutput.Should().Be("0");
        }

        [Fact]
        public void Equals_DefaultWarnTypeAndWind_ReturnsFalse()
        {
            // Arrange
            var warnType = default(WarnType);

            // Act
            var equals = warnType.Equals(WarnType.Wind);

            // Assert
            equals.Should().BeFalse();
        }

        [Fact]
        public void CompareTo_BoxedWarnType_ComparesValues()
        {
            // Arrange
            IComparable warnType = WarnType.Wind;

            // Act
            var result = warnType.CompareTo(WarnType.Rain);

            // Assert
            result.Should().BeNegative();
        }

        [Fact]
        public void CompareTo_BoxedInt_ComparesValues()
        {
            // Arrange
            IComparable warnType = WarnType.Rain;

            // Act
            var result = warnType.CompareTo(2);

            // Assert
            result.Should().Be(0);
        }

        [Fact]
        public void CompareTo_Null_ReturnsPositive()
        {
            // Arrange
            IComparable warnType = WarnType.Wind;

            // Act
            var result = warnType.CompareTo(null);

            // Assert
            result.Should().BePositive();
        }

        [Fact]
        public void CompareTo_OtherType_ThrowsArgumentException()
        {
            // Arrange
            IComparable warnType = WarnType.Wind;

            // Act
            Action action = () => warnType.CompareTo("Wind");

            // Assert
            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void TryGetFromValue_UnknownValue_ReturnsFalse()
        {
            // Act
            var success = WarnType.TryGetFromValue(13, out var warnType);

            // Assert
            success.Should().BeFalse();
            warnType.Should().Be(default(WarnType));
        }

        [Fact]
        public void ExplicitConversion_KnownValue_ReturnsWarnType()
        {
            // Act
            var warnType = (WarnType)12;

            // Assert
            warnType.Should().Be(WarnType.Drought);
        }

        [Fact]
        public void ExplicitConversion_UnknownValue_ThrowsArgumentOutOfRangeException()
        {
            // Act
            Action action = () => _ = (WarnType)13;

            // Assert
            action.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void Equals_SameValue_ReturnsTrueWithSameHashCode()
        {
            // Arrange
            var warnType = WarnType.FromValue(2);
            object boxed = WarnType.Rain;

            // Act
            var equalsObject = warnType.Equals(boxed);
            var equalsOperator = warnType == WarnType.Rain;
            var differsOperator = warnType != WarnType.Snow;

            // Assert
            equalsObject.Should().BeTrue();
            equalsOperator.Should().BeTrue();
            differsOperator.Should().BeTrue();
            warnType.GetHashCode().Should().Be(WarnType.Rain.GetHashCode());
        }

        [Fact]
        public void ComparisonOperators_Always_CompareValues()
        {
            // Act
            var less = WarnType.Wind < WarnType.Rain;
            var lessOrEqual = WarnType.Rain <= WarnType.Snow;
            var greater = WarnType.Drought > WarnType.Floods;
            var greaterOrEqual = WarnType.Floods >= WarnType.Drought;

            // Assert
            less.Should().BeTrue();
            lessOrEqual.Should().BeTrue();
            greater.Should().BeTrue();
            greaterOrEqual.Should().BeFalse();
        }

        [Fact]
        public void All_Always_IsReadOnly()
        {
            // Act
            var all = WarnType.All;

            // Assert
            all.Should().BeOfType<ReadOnlyCollection<WarnType>>();
            all.Should().HaveCount(13);
            all.Should().BeInAscendingOrder(w => w.Value);
        }
        [Fact]
        public void CompareTo_DefaultWarnTypeAndWind_ReturnsNonZero()
        {
            // Arrange
            var defaultWarnType = default(WarnType);

            // Act
            var comparison = defaultWarnType.CompareTo(WarnType.Wind);

            // Assert
            comparison.Should().NotBe(0);
        }

        [Fact]
        public void SortedSet_DefaultWarnTypeAndWind_HoldsBoth()
        {
            // Act
            var warnTypes = new SortedSet<WarnType> { default, WarnType.Wind };

            // Assert
            warnTypes.Should().HaveCount(2);
        }
    }
}
