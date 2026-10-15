using System.Collections.ObjectModel;

namespace MeteoSwissApi.Tests.Models
{
    [Trait(Traits.Category, Traits.UnitTests)]
    public class WarnLevelTests
    {
        [Fact]
        public void FromValue_Zero_ReturnsNoWarnLevel()
        {
            // Act
            var warnLevel = WarnLevel.FromValue(0);

            // Assert
            warnLevel.Should().Be(WarnLevel.NoWarnLevel);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-50)]
        public void FromValue_NegativeLevel_ThrowsArgumentOutOfRangeException(int warnLevelValue)
        {
            // Act
            Action action = () => WarnLevel.FromValue(warnLevelValue);

            // Assert
            action.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void TryGetFromValue_NegativeLevel_ReturnsFalse()
        {
            // Act
            var success = WarnLevel.TryGetFromValue(-7, out var warnLevel);

            // Assert
            success.Should().BeFalse();
            warnLevel.Should().Be(WarnLevel.NoWarnLevel);
        }

        [Theory]
        [ClassData(typeof(WarnLevelTestData))]
        public void FromValue_KnownLevel_ReturnsWarnLevel(int warnLevelValue, WarnLevel expectedWarnLevel)
        {
            // Act
            var warnLevel = WarnLevel.FromValue(warnLevelValue);

            // Assert
            warnLevel.Should().Be(expectedWarnLevel);
        }

        public class WarnLevelTestData : TheoryData<int, WarnLevel>
        {
            public WarnLevelTestData()
            {
                this.Add(1, WarnLevel.Level1);
                this.Add(2, WarnLevel.Level2);
                this.Add(3, WarnLevel.Level3);
                this.Add(4, WarnLevel.Level4);
                this.Add(5, WarnLevel.Level5);
            }
        }

        [Theory]
        [InlineData(6)]
        [InlineData(7)]
        [InlineData(8)]
        public void FromValue_LevelAboveMaxValue_ThrowsArgumentOutOfRangeException(int warnLevelValue)
        {
            // Act
            Action action = () => WarnLevel.FromValue(warnLevelValue);

            // Assert
            action.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Theory]
        [InlineData("en", 1, "Low")]
        [InlineData("en", 3, "Considerable")]
        [InlineData("en", 5, "Extreme")]
        [InlineData("de", 1, "Niedrig")]
        [InlineData("de", 3, "Beträchtlich")]
        [InlineData("fr", 3, "Considérable")]
        public void ToString_UICulture_ReturnsTextInUILanguage(string cultureName, int warnLevelValue, string expectedText)
        {
            // Arrange
            using var cultureScope = new CultureScope(cultureName);
            var warnLevel = WarnLevel.FromValue(warnLevelValue);

            // Act
            var text = warnLevel.ToString();

            // Assert
            text.Should().Be(expectedText);
        }

        [Fact]
        public void ToString_UICultureDiffersFromCulture_UsesUICulture()
        {
            // Arrange
            // The culture formats numbers and dates; the UI culture selects the language of texts.
            using var cultureScope = new CultureScope("de-CH", "en");

            // Act
            var text = WarnLevel.Level3.ToString();

            // Assert
            text.Should().Be("Considerable");
        }

        [Fact]
        public void ToString_CultureInfoProvider_ReturnsTextInThatLanguage()
        {
            // Arrange
            using var cultureScope = new CultureScope("en");

            // Act
            var text = WarnLevel.Level3.ToString(null, new CultureInfo("de"));

            // Assert
            text.Should().Be("Beträchtlich");
        }

        [Fact]
        public void ToString_FormatProviderIsNoCulture_ReturnsText()
        {
            // Arrange
            using var cultureScope = new CultureScope("en");

            // Act
            var text = string.Format(NumberFormatInfo.InvariantInfo, "{0}", WarnLevel.Level3);

            // Assert
            text.Should().Be("Considerable");
        }

        [Fact]
        public void ToString_FormatN_ReturnsLevel()
        {
            // Act
            var text = WarnLevel.Level3.ToString("N");

            // Assert
            text.Should().Be("3");
        }

        [Fact]
        public void TryGetFromValue_LevelAboveMaxValue_ReturnsFalse()
        {
            // Act
            var success = WarnLevel.TryGetFromValue(6, out var warnLevel);

            // Assert
            success.Should().BeFalse();
            warnLevel.Should().Be(WarnLevel.NoWarnLevel);
        }

        [Fact]
        public void ExplicitConversion_KnownLevel_ReturnsWarnLevel()
        {
            // Act
            var warnLevel = (WarnLevel)3;

            // Assert
            warnLevel.Should().Be(WarnLevel.Level3);
        }

        [Fact]
        public void ExplicitConversion_UnknownLevel_ThrowsArgumentOutOfRangeException()
        {
            // Act
            Action action = () => _ = (WarnLevel)6;

            // Assert
            action.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void ImplicitConversion_ToInt_ReturnsLevel()
        {
            // Act
            int level = WarnLevel.Level4;

            // Assert
            level.Should().Be(4);
        }

        [Fact]
        public void Equals_SameLevel_ReturnsTrueWithSameHashCode()
        {
            // Arrange
            var level = WarnLevel.FromValue(3);
            object boxed = WarnLevel.Level3;

            // Act
            var equalsObject = level.Equals(boxed);
            var equalsOperator = level == WarnLevel.Level3;
            var differsOperator = level != WarnLevel.Level4;

            // Assert
            equalsObject.Should().BeTrue();
            equalsOperator.Should().BeTrue();
            differsOperator.Should().BeTrue();
            level.GetHashCode().Should().Be(WarnLevel.Level3.GetHashCode());
        }

        [Fact]
        public void All_Always_IsReadOnly()
        {
            // Act
            var all = WarnLevel.All;

            // Assert
            all.Should().BeOfType<ReadOnlyCollection<WarnLevel>>();
            all.Should().Equal(WarnLevel.Level1, WarnLevel.Level2, WarnLevel.Level3, WarnLevel.Level4, WarnLevel.Level5);
        }
        [Fact]
        public void CompareTo_LowerLevel_ReturnsNegative()
        {
            // Act
            var comparison = WarnLevel.Level2.CompareTo(WarnLevel.Level3);

            // Assert
            comparison.Should().BeNegative();
        }

        [Fact]
        public void CompareTo_OtherType_ThrowsArgumentException()
        {
            // Act
            Action action = () => WarnLevel.Level2.CompareTo("Level3");

            // Assert
            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void ComparisonOperators_Always_CompareLevels()
        {
            // Assert
            (WarnLevel.Level1 < WarnLevel.Level2).Should().BeTrue();
            (WarnLevel.Level2 <= WarnLevel.Level3).Should().BeTrue();
            (WarnLevel.Level5 > WarnLevel.Level4).Should().BeTrue();
            (WarnLevel.NoWarnLevel >= WarnLevel.Level1).Should().BeFalse();
        }

        [Fact]
        public void OrderBy_Levels_SortsByLevel()
        {
            // Arrange
            var warnLevels = new[] { WarnLevel.Level3, WarnLevel.NoWarnLevel, WarnLevel.Level5, WarnLevel.Level1 };

            // Act
            var sorted = warnLevels.OrderBy(l => l).ToArray();

            // Assert
            sorted.Should().Equal(WarnLevel.NoWarnLevel, WarnLevel.Level1, WarnLevel.Level3, WarnLevel.Level5);
        }

        [Fact]
        public void ToString_UnknownLevel_ReturnsNumber()
        {
            // Arrange
            var warnLevel = WarnLevel.FromValueOrUnknown(7);

            // Act
            var text = warnLevel.ToString();

            // Assert
            text.Should().Be("7");
        }
    }
}
