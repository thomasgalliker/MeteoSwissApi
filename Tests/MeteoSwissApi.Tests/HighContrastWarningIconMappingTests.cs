using System.Text.Json;

namespace MeteoSwissApi.Tests
{
    public partial class HighContrastWarningIconMappingTests
    {
        [Theory]
        [Trait(Traits.Category, Traits.UnitTests)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        public async Task GetIconAsync_WarnLevelWithIcon_ReturnsEmbeddedIcon(int warnLevel)
        {
            // Arrange
            var highContrastWarningIconMapping = new HighContrastWarningIconMapping();

            // Act
            using var stream = await highContrastWarningIconMapping.GetIconAsync(WarnLevel.FromValue(warnLevel));

            // Assert
            TestHelper.IsTransparentIcon(stream).Should().BeFalse();

            using var streamReader = new StreamReader(stream);
            (await streamReader.ReadToEndAsync()).Should().Contain("<svg");
        }

        [Theory]
        [Trait(Traits.Category, Traits.UnitTests)]
        [InlineData(0)] // WarnLevel.NoWarnLevel
        [InlineData(1)] // WarnLevel.Level1 has no warning icon
        public async Task GetIconAsync_WarnLevelWithoutIcon_ReturnsTransparentIcon(int warnLevel)
        {
            // Arrange
            var highContrastWarningIconMapping = new HighContrastWarningIconMapping();

            // Act
            using var stream = await highContrastWarningIconMapping.GetIconAsync(WarnLevel.FromValue(warnLevel));

            // Assert
            TestHelper.IsTransparentIcon(stream).Should().BeTrue();
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetIconAsync_UnknownWarnLevel_ReturnsTransparentIcon()
        {
            // Arrange
            // MeteoSwiss may send warn levels above 5, which have no icon.
            var warnLevel = JsonSerializer.Deserialize<WarningsOverview>("{\"warnType\":0,\"warnLevel\":6}")!.WarnLevel;
            var highContrastWarningIconMapping = new HighContrastWarningIconMapping();

            // Act
            using var stream = await highContrastWarningIconMapping.GetIconAsync(warnLevel);

            // Assert
            TestHelper.IsTransparentIcon(stream).Should().BeTrue();
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetIconAsync_Cancelled_ThrowsOperationCanceledException()
        {
            // Arrange
            using var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            var highContrastWarningIconMapping = new HighContrastWarningIconMapping();

            // Act
            Func<Task> action = () => highContrastWarningIconMapping.GetIconAsync(WarnLevel.Level3, cancellationTokenSource.Token);

            // Assert
            await action.Should().ThrowAsync<OperationCanceledException>();
        }
    }
}