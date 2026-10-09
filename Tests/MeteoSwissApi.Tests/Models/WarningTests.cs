namespace MeteoSwissApi.Tests.Models
{
    [Trait(Traits.Category, Traits.UnitTests)]
    public class WarningTests
    {
        [Fact]
        public void ToString_NewWarning_ReturnsText()
        {
            // Arrange
            var warning = new Warning();

            // Act
            var stringOutput = warning.ToString();

            // Assert
            stringOutput.Should().StartWith("WarnType=0, ");
        }
    }
}
