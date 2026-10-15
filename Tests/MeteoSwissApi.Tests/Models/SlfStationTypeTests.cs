using System.Collections.ObjectModel;
using System.Text.Json;

namespace MeteoSwissApi.Tests.Models
{
    [Trait(Traits.Category, Traits.UnitTests)]
    public class SlfStationTypeTests
    {
        [Theory]
        [InlineData("en", SlfStationType.Wind, "Wind station")]
        [InlineData("en", SlfStationType.SnowFlat, "Snow station")]
        [InlineData("de", SlfStationType.Wind, "Windmessstation")]
        [InlineData("de", SlfStationType.FlowCapt, "Schneeverwehungssensor")]
        public void ToString_UICulture_ReturnsTextInUILanguage(string cultureName, string stationTypeValue, string expectedText)
        {
            // Arrange
            using var cultureScope = new CultureScope(cultureName);
            var slfStationType = new SlfStationType(stationTypeValue);

            // Act
            var text = slfStationType.ToString();

            // Assert
            text.Should().Be(expectedText);
        }

        [Fact]
        public void ToString_FormatProviderIsNoCulture_ReturnsText()
        {
            // Arrange
            using var cultureScope = new CultureScope("en");

            // Act
            var text = string.Format(NumberFormatInfo.InvariantInfo, "{0}", new SlfStationType(SlfStationType.Wind));

            // Assert
            text.Should().Be("Wind station");
        }

        [Fact]
        public void ToString_UnknownValue_ReturnsValue()
        {
            // Arrange
            var slfStationType = new SlfStationType("RADAR");

            // Act
            var text = slfStationType.ToString();

            // Assert
            text.Should().Be("RADAR");
        }

        [Fact]
        public void ToString_OtherFormat_ReturnsValue()
        {
            // Act
            var text = new SlfStationType(SlfStationType.Wind).ToString("V");

            // Assert
            text.Should().Be("WIND");
        }

        [Fact]
        public void All_Always_IsReadOnly()
        {
            // Act
            var all = SlfStationType.All;

            // Assert
            all.Should().BeOfType<ReadOnlyCollection<SlfStationType>>();
            all.Select(t => t.Value).Should().Equal(
                SlfStationType.SnowFlat,
                SlfStationType.SnowSlope,
                SlfStationType.Wind,
                SlfStationType.Special,
                SlfStationType.FlowCapt,
                SlfStationType.Pluvio);
        }

        [Fact]
        public void Deserialize_JsonString_ReturnsStationType()
        {
            // Arrange
            const string json = "\"WIND\"";

            // Act
            var slfStationType = JsonSerializer.Deserialize<SlfStationType>(json);

            // Assert
            slfStationType.Value.Should().Be(SlfStationType.Wind);
        }

        [Fact]
        public void Serialize_StationType_ReturnsJsonString()
        {
            // Arrange
            var slfStationType = new SlfStationType(SlfStationType.SnowFlat);

            // Act
            var json = JsonSerializer.Serialize(slfStationType);

            // Assert
            json.Should().Be("\"SNOW_FLAT\"");
        }

        [Fact]
        public void Serialize_DefaultValue_RoundTripsAsNull()
        {
            // Arrange
            var slfStationType = default(SlfStationType);

            // Act
            var json = JsonSerializer.Serialize(slfStationType);
            var deserialized = JsonSerializer.Deserialize<SlfStationType>(json);

            // Assert
            json.Should().Be("null");
            deserialized.Value.Should().BeNull();
        }
        [Fact]
        public void Equals_SameValue_ReturnsTrue()
        {
            // Arrange
            var left = new SlfStationType(SlfStationType.Wind);
            var right = new SlfStationType(SlfStationType.Wind);

            // Act
            var equal = left.Equals(right);

            // Assert
            equal.Should().BeTrue();
            (left == right).Should().BeTrue();
            left.GetHashCode().Should().Be(right.GetHashCode());
        }

        [Fact]
        public void Equals_DifferentValue_ReturnsFalse()
        {
            // Arrange
            var left = new SlfStationType(SlfStationType.Wind);
            var right = new SlfStationType(SlfStationType.SnowFlat);

            // Act
            var equal = left.Equals(right);

            // Assert
            equal.Should().BeFalse();
            (left != right).Should().BeTrue();
        }

        [Fact]
        public void Equals_DefaultValues_ReturnsTrue()
        {
            // Act
            var equal = default(SlfStationType).Equals(default);

            // Assert
            equal.Should().BeTrue();
        }

        [Fact]
        public void ExplicitConversion_String_ReturnsStationType()
        {
            // Act
            var stationType = (SlfStationType)SlfStationType.Pluvio;

            // Assert
            stationType.Value.Should().Be(SlfStationType.Pluvio);
        }

        [Fact]
        public void EqualityOperator_StringConstant_ComparesTheValue()
        {
            // Arrange
            var stationType = new SlfStationType(SlfStationType.Wind);

            // Act
            var isWind = stationType == SlfStationType.Wind;

            // Assert
            isWind.Should().BeTrue();
        }
    }
}
