namespace MeteoSwissApi.Tests.Models
{
    [Trait(Traits.Category, Traits.UnitTests)]
    public class RangeTests
    {
        [Theory]
        [InlineData(0, true)]
        [InlineData(5, true)]
        [InlineData(10, true)]
        [InlineData(-1, false)]
        [InlineData(11, false)]
        public void InRange_InclusiveBounds_IncludesBounds(int value, bool expectedInRange)
        {
            // Arrange
            var range = new Range<int>(0, 10);

            // Act
            var inRange = range.InRange(value);

            // Assert
            inRange.Should().Be(expectedInRange);
        }

        [Theory]
        [InlineData(0, false)]
        [InlineData(1, true)]
        [InlineData(9, true)]
        [InlineData(10, false)]
        public void InRange_ExclusiveBounds_ExcludesBounds(int value, bool expectedInRange)
        {
            // Arrange
            var range = new Range<int>(0, 10, minInclusive: false, maxInclusive: false);

            // Act
            var inRange = range.InRange(value);

            // Assert
            inRange.Should().Be(expectedInRange);
        }

        [Fact]
        public void IsEmpty_MinGreaterThanMax_ReturnsTrue()
        {
            // Arrange
            var range = new Range<int>(10, 0);

            // Act
            var isEmpty = range.IsEmpty;

            // Assert
            isEmpty.Should().BeTrue();
        }

        [Fact]
        public void IsEmpty_EqualInclusiveBounds_ReturnsFalse()
        {
            // Arrange
            // The range contains exactly one value.
            var range = new Range<int>(5, 5);

            // Act
            var isEmpty = range.IsEmpty;

            // Assert
            isEmpty.Should().BeFalse();
        }

        [Fact]
        public void IsEmpty_EqualBoundsOneExclusive_ReturnsTrue()
        {
            // Arrange
            var range = new Range<int>(5, 5, minInclusive: true, maxInclusive: false);

            // Act
            var isEmpty = range.IsEmpty;

            // Assert
            isEmpty.Should().BeTrue();
        }

        [Fact]
        public void IsEmpty_MinLessThanMax_ReturnsFalse()
        {
            // Arrange
            var range = new Range<int>(0, 10, minInclusive: false, maxInclusive: false);

            // Act
            var isEmpty = range.IsEmpty;

            // Assert
            isEmpty.Should().BeFalse();
        }

        [Fact]
        public void Constructor_MinIsNull_ThrowsArgumentNullException()
        {
            // Act
            Action action = () => _ = new Range<string>(null!, "b");

            // Assert
            action.Should().Throw<ArgumentNullException>().WithParameterName("min");
        }

        [Fact]
        public void ToString_MixedBounds_ReturnsIntervalNotation()
        {
            // Arrange
            var range = new Range<int>(0, 10, minInclusive: true, maxInclusive: false);

            // Act
            var text = range.ToString();

            // Assert
            text.Should().Be("[0, 10)");
        }
        [Fact]
        public void InRange_ValueIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            var range = new Range<string>("a", "c");

            // Act
            Action action = () => range.InRange(null!);

            // Assert
            action.Should().Throw<ArgumentNullException>().WithParameterName("value");
        }

        [Fact]
        public void ToString_FormatProvider_FormatsTheBoundsWithIt()
        {
            // Arrange
            var range = new Range<double>(0.5, 1.5);

            // Act
            var text = range.ToString(CultureInfo.GetCultureInfo("de-DE"));

            // Assert
            text.Should().Be("[0,5, 1,5]");
        }
    }
}
