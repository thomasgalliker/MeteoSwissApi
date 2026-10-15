using System.Text.Json;
using MeteoSwissApi.Models.Converters;

namespace MeteoSwissApi.Tests.Models.Converters
{
    [Trait(Traits.Category, Traits.UnitTests)]
    public class UtcDateTimeJsonConverterTests
    {
        private static readonly DateTime ExpectedTimestamp = new DateTime(2026, 10, 3, 9, 30, 0, DateTimeKind.Utc);

        [Theory]
        [InlineData("\"2026-10-03T09:30:00Z\"")]
        [InlineData("\"2026-10-03T11:30:00+02:00\"")]
        [InlineData("\"2026-10-03T09:30:00\"")]
        public void Read_Timestamp_ReturnsUtcDateTime(string json)
        {
            // Arrange
            var options = CreateOptions();

            // Act
            var timestamp = JsonSerializer.Deserialize<DateTime>(json, options);

            // Assert
            timestamp.Should().Be(ExpectedTimestamp);
            timestamp.Kind.Should().Be(DateTimeKind.Utc);
        }

        [Theory]
        [InlineData("1784477400000")]
        [InlineData("\"yesterday\"")]
        public void Read_NoTimestamp_ThrowsJsonException(string json)
        {
            // Arrange
            var options = CreateOptions();

            // Act
            Action action = () => JsonSerializer.Deserialize<DateTime>(json, options);

            // Assert
            action.Should().Throw<JsonException>();
        }

        [Fact]
        public void Read_NullableDateTime_ReturnsNull()
        {
            // Arrange
            var options = CreateOptions();

            // Act
            var timestamp = JsonSerializer.Deserialize<DateTime?>("null", options);

            // Assert
            timestamp.Should().BeNull();
        }

        [Fact]
        public void Write_UtcDateTime_WritesTimestampWithSuffixZ()
        {
            // Arrange
            var options = CreateOptions();

            // Act
            var json = JsonSerializer.Serialize(ExpectedTimestamp, options);

            // Assert
            json.Should().Be("\"2026-10-03T09:30:00Z\"");
        }

        private static JsonSerializerOptions CreateOptions()
        {
            return new JsonSerializerOptions { Converters = { new UtcDateTimeJsonConverter() } };
        }
        [Fact]
        public void Write_UnspecifiedKind_WritesTheSameTimeAsUtc()
        {
            // Arrange
            var options = new JsonSerializerOptions { Converters = { new UtcDateTimeJsonConverter() } };
            var timestamp = new DateTime(2026, 10, 3, 9, 30, 0, DateTimeKind.Unspecified);

            // Act
            var json = JsonSerializer.Serialize(timestamp, options);

            // Assert
            json.Should().Be("\"2026-10-03T09:30:00Z\"");
        }
    }
}
