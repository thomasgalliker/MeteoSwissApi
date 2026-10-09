using System.Text.Json;
using MeteoSwissApi.Models.Converters;

namespace MeteoSwissApi.Tests.Models.Converters
{
    [Trait(Traits.Category, Traits.UnitTests)]
    public class SlfStationTypeJsonConverterTests
    {
        private static JsonSerializerOptions CreateOptions()
        {
            return new JsonSerializerOptions { Converters = { new SlfStationTypeJsonConverter() } };
        }

        [Fact]
        public void Read_String_ReturnsStationTypeWithValue()
        {
            // Arrange
            var options = CreateOptions();

            // Act
            var stationType = JsonSerializer.Deserialize<SlfStationType>("\"SNOW_FLAT\"", options);

            // Assert
            stationType.Value.Should().Be("SNOW_FLAT");
        }

        [Fact]
        public void Read_Null_ReturnsDefault()
        {
            // Arrange
            var options = CreateOptions();

            // Act
            var stationType = JsonSerializer.Deserialize<SlfStationType>("null", options);

            // Assert
            stationType.Value.Should().BeNull();
        }

        [Theory]
        [InlineData("1")]
        [InlineData("true")]
        [InlineData("{}")]
        public void Read_NotAString_ThrowsJsonException(string json)
        {
            // Arrange
            var options = CreateOptions();

            // Act
            Action action = () => JsonSerializer.Deserialize<SlfStationType>(json, options);

            // Assert
            action.Should().Throw<JsonException>();
        }

        [Fact]
        public void Write_StationType_WritesString()
        {
            // Arrange
            var options = CreateOptions();

            // Act
            var json = JsonSerializer.Serialize(new SlfStationType("WIND"), options);

            // Assert
            json.Should().Be("\"WIND\"");
        }

        [Fact]
        public void Write_Default_WritesNull()
        {
            // Arrange
            var options = CreateOptions();
            var stationType = default(SlfStationType);

            // Act
            var json = JsonSerializer.Serialize(stationType, options);

            // Assert
            json.Should().Be("null");
        }
    }
}
