using System.Globalization;
using MeteoSwissApi.Models;
using Xunit;

namespace MeteoSwissApi.Tests.Models
{
    public class WarningsOverviewTests
    {
        [Fact]
        public void ShouldDoSomething_WhenCondition_ExpectedResult()
        {
            // Arrange
            var cultureInfo = new CultureInfo("de");

            var warningOverview = new WarningsOverview
            {
                WarnType = WarnType.Avalanches,
                WarnLevel = WarnLevel.Level1,
            };

            // Act
            var warnTypeAndLevel = warningOverview.WarnType.ToString(warningOverview.WarnLevel, cultureInfo);

            // Assert
            // TODO
        }
    }
}
