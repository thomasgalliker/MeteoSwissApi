using System.Text.Json;
using MeteoSwissApi.Models.Converters;

namespace MeteoSwissApi.Tests.Models.Converters
{
    [Trait(Traits.Category, Traits.UnitTests)]
    public class QuantityJsonConverterTests
    {
        [Theory]
        [InlineData("12.5", 12.5)]
        [InlineData("\"12.5\"", 12.5)]
        [InlineData("-3", -3)]
        public void Read_NumberOrNumericString_ReturnsQuantityInUnitOfConverter(string json, double expectedMillimeters)
        {
            // Arrange
            var options = CreateOptions(new MillimeterLengthJsonConverter());

            // Act
            var length = JsonSerializer.Deserialize<Length>(json, options);

            // Assert
            length.Should().Be(Length.FromMillimeters(expectedMillimeters));
        }

        [Theory]
        [InlineData("null")]
        [InlineData("true")]
        [InlineData("\"-\"")]
        [InlineData("\"NaN\"")]
        [InlineData("\"Infinity\"")]
        public void Read_NoNumber_ReturnsDefaultQuantity(string json)
        {
            // Arrange
            var options = CreateOptions(new MillimeterLengthJsonConverter());

            // Act
            var length = JsonSerializer.Deserialize<Length>(json, options);

            // Assert
            length.Should().Be(default);
        }

        [Fact]
        public void Write_QuantityInOtherUnit_WritesNumberInUnitOfConverter()
        {
            // Arrange
            var options = CreateOptions(new MillimeterLengthJsonConverter());

            // Act
            var json = JsonSerializer.Serialize(Length.FromMeters(1), options);

            // Assert
            json.Should().Be("1000");
        }

        [Fact]
        public void RoundTrip_QuantityInOtherUnit_ReturnsEqualQuantity()
        {
            // Arrange
            var options = CreateOptions(new MillimeterLengthJsonConverter());
            var length = Length.FromMeters(1);

            // Act
            var roundTripped = JsonSerializer.Deserialize<Length>(JsonSerializer.Serialize(length, options), options);

            // Assert
            roundTripped.Meters.Should().BeApproximately(1, 1e-9);
        }

        [Theory]
        [ClassData(typeof(ConverterUnitTestData))]
        public void Write_One_WritesOneInUnitOfConverter(System.Text.Json.Serialization.JsonConverter converter, object quantity, string expectedJson)
        {
            // Arrange
            var options = CreateOptions(converter);

            // Act
            var json = JsonSerializer.Serialize(quantity, quantity.GetType(), options);

            // Assert
            json.Should().Be(expectedJson);
        }

        public class ConverterUnitTestData : TheoryData<System.Text.Json.Serialization.JsonConverter, object, string>
        {
            public ConverterUnitTestData()
            {
                this.Add(new CentimeterLengthJsonConverter(), Length.FromCentimeters(1), "1");
                this.Add(new MeterLengthJsonConverter(), Length.FromMeters(1), "1");
                this.Add(new KilometerLengthJsonConverter(), Length.FromKilometers(1), "1");
                this.Add(new MinuteDurationJsonConverter(), Duration.FromMinutes(1), "1");
                this.Add(new HourDurationJsonConverter(), Duration.FromHours(1), "1");
                this.Add(new PercentRatioJsonConverter(), Ratio.FromPercent(1), "1");
                this.Add(new DecimalFractionRatioJsonConverter(), Ratio.FromDecimalFractions(1), "1");
                this.Add(new SpeedJsonConverter(), Speed.FromKilometersPerHour(1), "1");
                this.Add(new DegreeAngleJsonConverter(), Angle.FromDegrees(1), "1");
                this.Add(new TemperatureJsonConverter(), Temperature.FromDegreesCelsius(1), "1");
                this.Add(new TemperatureDeltaJsonConverter(), TemperatureDelta.FromDegreesCelsius(1), "1");
            }
        }

        private static JsonSerializerOptions CreateOptions(System.Text.Json.Serialization.JsonConverter converter)
        {
            return new JsonSerializerOptions { Converters = { converter } };
        }
    }
}
