using System.Text.Json;
using MeteoSwissApi.Models.Converters;

namespace MeteoSwissApi.Tests.Models.Converters
{
    [Trait(Traits.Category, Traits.UnitTests)]
    public class EmptyArrayJsonConverterTests
    {
        [Fact]
        public void Read_Null_ReturnsEmptyArray()
        {
            // Arrange
            var options = new JsonSerializerOptions { Converters = { new EmptyArrayJsonConverter<string>() } };

            // Act
            var values = JsonSerializer.Deserialize<string[]>("null", options);

            // Assert
            values.Should().BeEmpty();
        }

        [Fact]
        public void Read_NullElementOfReferenceType_SkipsTheElement()
        {
            // Arrange
            var options = new JsonSerializerOptions { Converters = { new EmptyArrayJsonConverter<string>() } };

            // Act
            var values = JsonSerializer.Deserialize<string[]>("[null,\"a\",null]", options);

            // Assert
            values.Should().Equal("a");
        }

        [Fact]
        public void Read_NullElementOfValueType_ThrowsJsonException()
        {
            // Arrange
            var options = new JsonSerializerOptions { Converters = { new EmptyArrayJsonConverter<int>() } };

            // Act
            Action action = () => JsonSerializer.Deserialize<int[]>("[1,null]", options);

            // Assert
            action.Should().Throw<JsonException>();
        }

        [Fact]
        public void Read_NoArray_ThrowsJsonException()
        {
            // Arrange
            var options = new JsonSerializerOptions { Converters = { new EmptyArrayJsonConverter<string>() } };

            // Act
            Action action = () => JsonSerializer.Deserialize<string[]>("\"a\"", options);

            // Assert
            action.Should().Throw<JsonException>();
        }

        [Fact]
        public void Write_Array_WritesTheElements()
        {
            // Arrange
            var options = new JsonSerializerOptions { Converters = { new EmptyArrayJsonConverter<int>() } };

            // Act
            var json = JsonSerializer.Serialize(new[] { 1, 2 }, options);

            // Assert
            json.Should().Be("[1,2]");
        }
    }
}
