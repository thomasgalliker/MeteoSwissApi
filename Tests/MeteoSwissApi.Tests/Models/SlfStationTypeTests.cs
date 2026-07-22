using System.Text.Json;

namespace MeteoSwissApi.Tests.Models
{
    [Trait(Traits.Category, Traits.UnitTests)]
    public class SlfStationTypeTests
    {
        private readonly ITestOutputHelper testOutputHelper;

        public SlfStationTypeTests(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        [Theory]
        [ClassData(typeof(ToStringTestData))]

        public void ShouldGetToString(SlfStationType slfStationType)
        {
            // Act
            var stringOutput = slfStationType.ToString();

            // Assert
            this.testOutputHelper.WriteLine($"stringOutput={stringOutput}");
            stringOutput.Should().NotBeNullOrEmpty();
        }

        private class ToStringTestData : TheoryData<SlfStationType>
        {
            public ToStringTestData()
            {
                foreach (var warnTypeRange in SlfStationType.All)
                {
                    this.Add(warnTypeRange);
                }
            }
        }

        [Fact]
        public void ShouldDeserializeFromJsonString()
        {
            // Arrange
            const string json = "\"WIND\"";

            // Act
            var slfStationType = JsonSerializer.Deserialize<SlfStationType>(json);

            // Assert
            slfStationType.Value.Should().Be(SlfStationType.Wind);
        }

        [Fact]
        public void ShouldSerializeToJsonString()
        {
            // Arrange
            var slfStationType = new SlfStationType(SlfStationType.SnowFlat);

            // Act
            var json = JsonSerializer.Serialize(slfStationType);

            // Assert
            json.Should().Be("\"SNOW_FLAT\"");
        }

        [Fact]
        public void ShouldRoundTripDefaultValue()
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
    }
}
