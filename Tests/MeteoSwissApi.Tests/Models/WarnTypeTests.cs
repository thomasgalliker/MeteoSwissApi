using System;
using FluentAssertions;
using MeteoSwissApi.Models;
using Xunit;
using Xunit.Abstractions;

namespace MeteoSwissApi.Tests.Models
{
    public class WarnTypeTests
    {
        private readonly ITestOutputHelper testOutputHelper;

        public WarnTypeTests(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        [Theory]
        [ClassData(typeof(WarnTypeTestData))]

        public void ShouldGetFromValue(int warnTypeValue, WarnType expectedWarnType)
        {
            // Act
            var warnType = WarnType.FromValue(warnTypeValue);

            // Assert
            warnType.Should().Be(expectedWarnType);
        }

        public class WarnTypeTestData : TheoryData<int, WarnType>
        {
            public WarnTypeTestData()
            {
                this.Add(0, WarnType.Wind);
                this.Add(1, WarnType.Thunderstorms);
                this.Add(2, WarnType.Rain);
            }
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(12)]
        public void ShouldThrowOutOfRangeException(int warnTypeValue)
        {
            // Act
            Action action = () => WarnType.FromValue(warnTypeValue);

            // Assert
            action.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Theory]
        [ClassData(typeof(WarnTypeToStringTestData))]

        public void ShouldGetToString(WarnType warnType)
        {
            // Act
            var stringOutput = warnType.ToString();

            // Assert
            this.testOutputHelper.WriteLine($"stringOutput={stringOutput}");
            stringOutput.Should().NotBeNullOrEmpty();
        }

        public class WarnTypeToStringTestData : TheoryData<WarnType>
        {
            public WarnTypeToStringTestData()
            {
                foreach (var warnTypeRange in WarnType.All)
                {
                    this.Add(warnTypeRange);
                }
            }
        }
    }
}