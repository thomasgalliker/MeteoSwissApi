using System.Text.Json;
using MeteoSwissApi.Models.Converters;

namespace MeteoSwissApi.Tests.Models.Converters
{
    [Trait(Traits.Category, Traits.UnitTests)]
    public class EpochDateTimeJsonConverterTests
    {
        private const long Milliseconds = 1784477400000;

        private static readonly DateTime ExpectedTimestamp = new DateTime(2026, 7, 19, 16, 10, 0, DateTimeKind.Utc);

        [Fact]
        public void Read_Milliseconds_ReturnsUtcDateTime()
        {
            // Arrange
            var options = CreateOptions(new EpochDateTimeJsonConverter());

            // Act
            var timestamp = JsonSerializer.Deserialize<DateTime>(Milliseconds.ToString(CultureInfo.InvariantCulture), options);

            // Assert
            timestamp.Should().Be(ExpectedTimestamp);
            timestamp.Kind.Should().Be(DateTimeKind.Utc);
        }

        [Fact]
        public void Write_UtcDateTime_WritesMilliseconds()
        {
            // Arrange
            var options = CreateOptions(new EpochDateTimeJsonConverter());

            // Act
            var json = JsonSerializer.Serialize(ExpectedTimestamp, options);

            // Assert
            json.Should().Be("1784477400000");
        }

        [Fact]
        public void Write_LocalDateTime_WritesMillisecondsOfSameInstant()
        {
            // Arrange
            var options = CreateOptions(new EpochDateTimeJsonConverter());

            // Act
            var json = JsonSerializer.Serialize(ExpectedTimestamp.ToLocalTime(), options);

            // Assert
            json.Should().Be("1784477400000");
        }

        [Fact]
        public void Read_ObsoletePublicConverter_ReturnsSameDateTime()
        {
            // Arrange
#pragma warning disable CS0618 // The public converter stays until it is removed; it must keep working meanwhile.
            var options = CreateOptions(new EpochDateTimeConverter());
#pragma warning restore CS0618

            // Act
            var timestamp = JsonSerializer.Deserialize<DateTime>(Milliseconds.ToString(CultureInfo.InvariantCulture), options);

            // Assert
            timestamp.Should().Be(ExpectedTimestamp);
        }

        private static JsonSerializerOptions CreateOptions(System.Text.Json.Serialization.JsonConverter converter)
        {
            return new JsonSerializerOptions { Converters = { converter } };
        }
        [Fact]
        public void Write_MinValue_WritesNull()
        {
            // Arrange
            var options = CreateOptions(new EpochDateTimeJsonConverter());

            // Act
            var json = JsonSerializer.Serialize(DateTime.MinValue, options);

            // Assert
            json.Should().Be("null");
        }

        [Fact]
        public void Write_UnspecifiedKind_WritesMillisecondsAsUtc()
        {
            // Arrange
            var options = CreateOptions(new EpochDateTimeJsonConverter());
            var timestamp = new DateTime(2026, 7, 19, 16, 10, 0, DateTimeKind.Unspecified);

            // Act
            var json = JsonSerializer.Serialize(timestamp, options);

            // Assert
            json.Should().Be("1784477400000");
        }

        [Theory]
        [InlineData("\"1784477400000\"")]
        [InlineData("true")]
        public void Read_NoNumber_ThrowsJsonException(string json)
        {
            // Arrange
            var options = CreateOptions(new EpochDateTimeJsonConverter());

            // Act
            Action action = () => JsonSerializer.Deserialize<DateTime>(json, options);

            // Assert
            action.Should().Throw<JsonException>();
        }
    }
}
