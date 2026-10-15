namespace MeteoSwissApi.Tests.Models
{
    [Trait(Traits.Category, Traits.UnitTests)]
    public class WarningsOverviewTests
    {
        [Theory]
        [InlineData("en", "WarnType=Avalanches, WarnLevel=Low")]
        [InlineData("de", "WarnType=Lawinen, WarnLevel=Niedrig")]
        public void ToString_AvalanchesLevel1_ReturnsTypeAndLevel(string cultureName, string expectedText)
        {
            // Arrange
            using var cultureScope = new CultureScope(cultureName);

            var warningsOverview = new WarningsOverview
            {
                WarnType = WarnType.Avalanches,
                WarnLevel = WarnLevel.Level1,
            };

            // Act
            var text = warningsOverview.ToString();

            // Assert
            text.Should().Be(expectedText);
        }
    }
}
