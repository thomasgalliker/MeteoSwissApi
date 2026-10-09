using System.Text.Json;
using MeteoSwissApi.Models.Converters;

namespace MeteoSwissApi.Tests.Models.Converters
{
    [Trait(Traits.Category, Traits.UnitTests)]
    public class WarnTypeJsonConverterTests
    {
        private static JsonSerializerOptions CreateOptions()
        {
            return new JsonSerializerOptions { Converters = { new WarnTypeJsonConverter() } };
        }

        [Fact]
        public void Read_KnownValue_ReturnsWarnType()
        {
            // Arrange
            var options = CreateOptions();

            // Act
            var warnType = JsonSerializer.Deserialize<WarnType>("12", options);

            // Assert
            warnType.Should().Be(WarnType.Drought);
        }

        [Fact]
        public void Read_UnknownValue_KeepsValue()
        {
            // Arrange
            var options = CreateOptions();

            // Act
            var warnType = JsonSerializer.Deserialize<WarnType>("99", options);

            // Assert
            warnType.Value.Should().Be(99);
        }

        [Fact]
        public void Read_Null_ReturnsDefault()
        {
            // Arrange
            var options = CreateOptions();

            // Act
            var warnType = JsonSerializer.Deserialize<WarnType>("null", options);

            // Assert
            warnType.Should().Be(default(WarnType));
        }

        [Fact]
        public void Read_String_ThrowsJsonException()
        {
            // Arrange
            var options = CreateOptions();

            // Act
            Action action = () => JsonSerializer.Deserialize<WarnType>("\"Wind\"", options);

            // Assert
            action.Should().Throw<JsonException>();
        }
    }
}
