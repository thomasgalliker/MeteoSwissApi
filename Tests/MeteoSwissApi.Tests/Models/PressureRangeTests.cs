using System;
using FluentAssertions;
using MeteoSwissApi.Models;
using UnitsNet;
using Xunit;
using Xunit.Abstractions;

namespace MeteoSwissApi.Tests.Models
{
    public class PressureRangeTests
    {
        private readonly ITestOutputHelper testOutputHelper;

        public PressureRangeTests(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        [Theory]
        [ClassData(typeof(PressureRangeTestData))]

        public void ShouldGetFromValue(Pressure pressure, PressureRange expectedPressureRange)
        {
            // Act
            var pressureRange = PressureRange.FromValue(pressure);

            // Assert
            pressureRange.Should().Be(expectedPressureRange);
        }

        public class PressureRangeTestData : TheoryData<Pressure, PressureRange>
        {
            public PressureRangeTestData()
            {
                this.Add(Pressure.FromHectopascals(0), PressureRange.VeryLow);
                this.Add(Pressure.FromHectopascals(998), PressureRange.VeryLow);
                this.Add(Pressure.FromHectopascals(999), PressureRange.Low);
                this.Add(Pressure.FromHectopascals(1007), PressureRange.Low);
                this.Add(Pressure.FromHectopascals(1008), PressureRange.Average);
                this.Add(Pressure.FromHectopascals(1018), PressureRange.Average);
                this.Add(Pressure.FromHectopascals(1019), PressureRange.High);
                this.Add(Pressure.FromHectopascals(1027), PressureRange.High);
                this.Add(Pressure.FromHectopascals(1028), PressureRange.VeryHigh);
                this.Add(Pressure.FromHectopascals(1044), PressureRange.VeryHigh);
            }
        }

        [Fact]
        public void ShouldThrowOutOfRangeException()
        {
            // Arrange
            var pressure = Pressure.FromHectopascals(-1);

            // Act
            Action action = () => PressureRange.FromValue(pressure);

            // Assert
            action.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Theory]
        [ClassData(typeof(PressureRangeToStringTestData))]

        public void ShouldGetToString(PressureRange pressureRange)
        {
            // Act
            var stringOutput = pressureRange.ToString();

            // Assert
            this.testOutputHelper.WriteLine($"stringOutput={stringOutput}");
            stringOutput.Should().NotBeNullOrEmpty();
        }

        public class PressureRangeToStringTestData : TheoryData<PressureRange>
        {
            public PressureRangeToStringTestData()
            {
                foreach (var pressureRange in PressureRange.All)
                {
                    this.Add(pressureRange);
                }
            }
        }
    }
}