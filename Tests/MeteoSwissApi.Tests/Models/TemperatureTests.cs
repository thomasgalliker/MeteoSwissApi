using System;
using FluentAssertions;
using MeteoSwissApi.Models;
using Xunit;
using Xunit.Abstractions;

namespace MeteoSwissApi.Tests.Models
{
    public class TemperatureTests
    {
        private readonly ITestOutputHelper testOutputHelper;

        public TemperatureTests(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        [Theory]
        [ClassData(typeof(TemperatureTestData))]
        public void ShouldConvertToString(double value, TemperatureUnit unit, string format, string expectedStringOutput)
        {
            // Arrange
            var temperature = new Temperature(value, unit);

            // Act
            var stringOutput = temperature.ToString(format);

            // Assert
            this.testOutputHelper.WriteLine($"stringOutput={stringOutput}");

            temperature.Value.Should().Be(value);
            temperature.Unit.Should().Be(unit);

            stringOutput.Should().Be(expectedStringOutput);
        }

        public class TemperatureTestData : TheoryData<double, TemperatureUnit, string, string>
        {
            public TemperatureTestData()
            {
                // Default format
                this.Add(-1d, TemperatureUnit.Celsius, null, "-1°C");
                this.Add(-0.1d, TemperatureUnit.Celsius, null, "-0.1°C");
                this.Add(0d, TemperatureUnit.Celsius, null, "0°C");
                this.Add(1d, TemperatureUnit.Celsius, null, "1°C");
                this.Add(1.2d, TemperatureUnit.Celsius, null, "1.2°C");
                this.Add(1.23d, TemperatureUnit.Celsius, null, "1.23°C");
                this.Add(1.234d, TemperatureUnit.Celsius, null, "1.23°C");
                this.Add(1.2345d, TemperatureUnit.Celsius, null, "1.23°C");
                this.Add(99.99999999d, TemperatureUnit.Celsius, null, "100°C");

                // Format "N0"
                this.Add(-1d, TemperatureUnit.Celsius, "N0", "-1");
                this.Add(-0.1d, TemperatureUnit.Celsius, "N0", "0");
                this.Add(0d, TemperatureUnit.Celsius, "N0", "0");
                this.Add(1d, TemperatureUnit.Celsius, "N0", "1");
                this.Add(1.2d, TemperatureUnit.Celsius, "N0", "1");
                this.Add(1.23d, TemperatureUnit.Celsius, "N0", "1");
                this.Add(1.234d, TemperatureUnit.Celsius, "N0", "1");
                this.Add(1.2345d, TemperatureUnit.Celsius, "N0", "1");
                this.Add(99.99999999d, TemperatureUnit.Celsius, "N0", "100");
            }
        }

        [Fact]
        public void ShouldThrowOutOfRangeException_NegativeKelvin()
        {
            // Act
            Action action = () => new Temperature(-1d, TemperatureUnit.Kelvin);

            // Assert
            action.Should().Throw<ArgumentOutOfRangeException>();
        }
    }
}