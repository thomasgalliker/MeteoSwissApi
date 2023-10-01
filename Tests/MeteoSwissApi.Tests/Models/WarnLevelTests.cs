using System;
using FluentAssertions;
using MeteoSwissApi.Models;
using Xunit;
using Xunit.Abstractions;

namespace MeteoSwissApi.Tests.Models
{
    public class WarnLevelTests
    {
        private readonly ITestOutputHelper testOutputHelper;

        public WarnLevelTests(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-50)]
        public void ShouldGetNoWarnLevel(int warnLevelValue)
        {
            // Act
            var warnLevel = WarnLevel.FromValue(warnLevelValue);

            // Assert
            warnLevel.Should().Be(WarnLevel.NoWarnLevel);
        }

        [Theory]
        [ClassData(typeof(WarnLevelTestData))]
        public void ShouldGetFromValue(int warnLevelValue, WarnLevel expectedWarnLevel)
        {
            // Act
            var warnLevel = WarnLevel.FromValue(warnLevelValue);

            // Assert
            warnLevel.Should().Be(expectedWarnLevel);
        }

        public class WarnLevelTestData : TheoryData<int, WarnLevel>
        {
            public WarnLevelTestData()
            {
                this.Add(1, WarnLevel.Level1);
                this.Add(2, WarnLevel.Level2);
                this.Add(3, WarnLevel.Level3);
                this.Add(4, WarnLevel.Level4);
                this.Add(5, WarnLevel.Level5);
            }
        }

        [Theory]
        [InlineData(6)]
        [InlineData(7)]
        [InlineData(8)]
        public void ShouldThrowOutOfRangeException(int warnLevelValue)
        {
            // Act
            Action action = () => WarnLevel.FromValue(warnLevelValue);

            // Assert
            action.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Theory]
        [ClassData(typeof(WarnLevelToStringTestData))]

        public void ShouldGetToString(WarnLevel warnLevel)
        {
            // Act
            var stringOutput = warnLevel.ToString();

            // Assert
            this.testOutputHelper.WriteLine($"stringOutput={stringOutput}");
            stringOutput.Should().NotBeNullOrEmpty();
        }

        public class WarnLevelToStringTestData : TheoryData<WarnLevel>
        {
            public WarnLevelToStringTestData()
            {
                foreach (var warnLevelRange in WarnLevel.All)
                {
                    this.Add(warnLevelRange);
                }
            }
        }
    }
}