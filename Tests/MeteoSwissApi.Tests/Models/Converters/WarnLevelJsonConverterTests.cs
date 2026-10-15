using System.Text.Json;
using MeteoSwissApi.Models.Converters;

namespace MeteoSwissApi.Tests.Models.Converters
{
    [Trait(Traits.Category, Traits.UnitTests)]
    public class WarnLevelJsonConverterTests
    {
        private static JsonSerializerOptions CreateOptions()
        {
            return new JsonSerializerOptions { Converters = { new WarnLevelJsonConverter() } };
        }

        [Fact]
        public void Read_KnownLevel_ReturnsWarnLevel()
        {
            // Arrange
            var options = CreateOptions();

            // Act
            var warnLevel = JsonSerializer.Deserialize<WarnLevel>("3", options);

            // Assert
            warnLevel.Should().Be(WarnLevel.Level3);
        }

        [Fact]
        public void Read_UnknownLevel_KeepsLevel()
        {
            // Arrange
            var options = CreateOptions();

            // Act
            var warnLevel = JsonSerializer.Deserialize<WarnLevel>("6", options);

            // Assert
            warnLevel.Level.Should().Be(6);
        }

        [Fact]
        public void Read_Null_ReturnsNoWarnLevel()
        {
            // Arrange
            var options = CreateOptions();

            // Act
            var warnLevel = JsonSerializer.Deserialize<WarnLevel>("null", options);

            // Assert
            warnLevel.Should().Be(WarnLevel.NoWarnLevel);
        }

        [Fact]
        public void Read_String_ThrowsJsonException()
        {
            // Arrange
            var options = CreateOptions();

            // Act
            Action action = () => JsonSerializer.Deserialize<WarnLevel>("\"Level3\"", options);

            // Assert
            action.Should().Throw<JsonException>();
        }
    }
}
