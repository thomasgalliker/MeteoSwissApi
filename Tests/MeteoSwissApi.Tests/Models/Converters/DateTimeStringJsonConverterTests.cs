using System.Text.Json;
using MeteoSwissApi.Models.Converters;

namespace MeteoSwissApi.Tests.Models.Converters
{
    [Trait(Traits.Category, Traits.UnitTests)]
    public class DateTimeStringJsonConverterTests
    {
        [Fact]
        public void Read_Date_ReturnsCalendarDateWithoutTimeZone()
        {
            // Arrange
            var options = CreateOptions();

            // Act
            var date = JsonSerializer.Deserialize<DateTime>("\"2026-07-19\"", options);

            // Assert
            date.Should().Be(new DateTime(2026, 7, 19));
            date.Kind.Should().Be(DateTimeKind.Unspecified);
            date.TimeOfDay.Should().Be(TimeSpan.Zero);
        }

        [Fact]
        public void Write_Date_WritesDateOnly()
        {
            // Arrange
            var options = CreateOptions();

            // Act
            var json = JsonSerializer.Serialize(new DateTime(2026, 7, 19), options);

            // Assert
            json.Should().Be("\"2026-07-19\"");
        }

        private static JsonSerializerOptions CreateOptions()
        {
            return new JsonSerializerOptions { Converters = { new DateTimeStringJsonConverter() } };
        }
        [Fact]
        public void Read_Null_ReturnsMinValue()
        {
            // Arrange
            var options = new JsonSerializerOptions { Converters = { new DateTimeStringJsonConverter() } };

            // Act
            var date = JsonSerializer.Deserialize<DateTime>("null", options);

            // Assert
            date.Should().Be(DateTime.MinValue);
        }

        [Theory]
        [InlineData("12345")]
        [InlineData("true")]
        [InlineData("\"2026-13-45\"")]
        [InlineData("\"19.07.2026\"")]
        public void Read_NoCalendarDate_ThrowsJsonException(string json)
        {
            // Arrange
            var options = new JsonSerializerOptions { Converters = { new DateTimeStringJsonConverter() } };

            // Act
            Action action = () => JsonSerializer.Deserialize<DateTime>(json, options);

            // Assert
            action.Should().Throw<JsonException>();
        }
    }
}
