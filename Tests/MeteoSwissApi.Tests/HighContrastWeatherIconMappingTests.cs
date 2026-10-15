namespace MeteoSwissApi.Tests
{
    public partial class HighContrastWeatherIconMappingTests
    {
        [Theory]
        [Trait(Traits.Category, Traits.UnitTests)]
        [InlineData(1)]
        [InlineData(142)]
        public async Task GetIconAsync_IconExists_ReturnsEmbeddedIcon(int iconId)
        {
            // Arrange
            var highContrastWeatherIconMapping = new HighContrastWeatherIconMapping();

            // Act
            using var stream = await highContrastWeatherIconMapping.GetIconAsync(iconId);

            // Assert
            TestHelper.IsTransparentIcon(stream).Should().BeFalse();

            using var streamReader = new StreamReader(stream);
            (await streamReader.ReadToEndAsync()).Should().Contain("<svg");
        }

        [Fact]
        [Trait(Traits.Category, Traits.UnitTests)]
        public async Task GetIconAsync_IconDoesNotExist_ReturnsTransparentIcon()
        {
            // Arrange
            var highContrastWeatherIconMapping = new HighContrastWeatherIconMapping();

            // Act
            using var stream = await highContrastWeatherIconMapping.GetIconAsync(999);

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
            var highContrastWeatherIconMapping = new HighContrastWeatherIconMapping();

            // Act
            Func<Task> action = () => highContrastWeatherIconMapping.GetIconAsync(1, cancellationTokenSource.Token);

            // Assert
            await action.Should().ThrowAsync<OperationCanceledException>();
        }
    }
}
