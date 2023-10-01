using System;
using FluentAssertions;
using MeteoSwissApi.Models;
using UnitsNet;
using Xunit;
using Xunit.Abstractions;

namespace MeteoSwissApi.Tests.Models
{
    public class HumidityRangeTests
    {
        private readonly ITestOutputHelper testOutputHelper;

        public HumidityRangeTests(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        [Theory]
        [ClassData(typeof(HumidityRangeTestData))]

        public void ShouldGetFromValue(RelativeHumidity humidity, HumidityRange expectedHumidityRange)
        {
            // Act
            var humidityRange = HumidityRange.FromValue(humidity);

            // Assert
            humidityRange.Should().Be(expectedHumidityRange);
        }

        public class HumidityRangeTestData : TheoryData<RelativeHumidity, HumidityRange>
        {
            public HumidityRangeTestData()
            {
                this.Add(RelativeHumidity.FromPercent(0), HumidityRange.VeryDry);
                this.Add(RelativeHumidity.FromPercent(30), HumidityRange.VeryDry);
                this.Add(RelativeHumidity.FromPercent(31), HumidityRange.Dry);
                this.Add(RelativeHumidity.FromPercent(39), HumidityRange.Dry);
                this.Add(RelativeHumidity.FromPercent(40), HumidityRange.Average);
                this.Add(RelativeHumidity.FromPercent(70), HumidityRange.Average);
                this.Add(RelativeHumidity.FromPercent(71), HumidityRange.Moist);
                this.Add(RelativeHumidity.FromPercent(79), HumidityRange.Moist);
                this.Add(RelativeHumidity.FromPercent(80), HumidityRange.VeryMoist);
                this.Add(RelativeHumidity.FromPercent(100), HumidityRange.VeryMoist);
            }
        }

        [Fact]
        public void ShouldThrowOutOfRangeException()
        {
            // Arrange
            var humidity = RelativeHumidity.FromPercent(120);

            // Act
            Action action = () => HumidityRange.FromValue(humidity);

            // Assert
            action.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Theory]
        [ClassData(typeof(HumidityRangeToStringTestData))]

        public void ShouldGetToString(HumidityRange humidityRange)
        {
            // Act
            var stringOutput = humidityRange.ToString();

            // Assert
            this.testOutputHelper.WriteLine($"stringOutput={stringOutput}");
            stringOutput.Should().NotBeNullOrEmpty();
        }

        public class HumidityRangeToStringTestData : TheoryData<HumidityRange>
        {
            public HumidityRangeToStringTestData()
            {
                foreach (var humidityRange in HumidityRange.All)
                {
                    this.Add(humidityRange);
                }
            }
        }
    }
}