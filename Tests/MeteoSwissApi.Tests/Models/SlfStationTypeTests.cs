using System;
using FluentAssertions;
using MeteoSwissApi.Models;
using Xunit;
using Xunit.Abstractions;

namespace MeteoSwissApi.Tests.Models
{
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

        public class ToStringTestData : TheoryData<SlfStationType>
        {
            public ToStringTestData()
            {
                foreach (var warnTypeRange in SlfStationType.All)
                {
                    this.Add(warnTypeRange);
                }
            }
        }
    }
}