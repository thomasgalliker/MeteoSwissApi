using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using MeteoSwissApi.Models;
using MeteoSwissApi.Tests.Utils;
using Xunit;

namespace MeteoSwissApi.Tests.Models
{
    public class WeatherConditionCodeTests
    {
        [Fact]
        public void ShouldShouldGetFromValue_NotExisting()
        {
            // Arrange
            const int value = -100;

            // Act
            Action action = () => WeatherConditionCode.FromValue(value);

            // Assert
            action.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public async Task ShouldGetFromValue_Existing()
        {
            // Arrange
            var range = Enumerable.Range(1, 200).ToArray();

            var weatherIconMapping = new HighContrastWeatherIconMapping();
            var downloadedIcons = await TestHelper.TryGetIconsAsync(range, weatherIconMapping.GetIconAsync);

            static (int Id, WeatherConditionCode WeatherConditionCode, bool Success) TryCreateWeatherConditionCode((int IconId, Stream Stream) i)
            {
                var success = WeatherConditionCode.TryGetFromValue(i.IconId, out var weatherConditionCode);
                return (i.IconId, weatherConditionCode, success);
            }

            // Act
            var weatherConditionCodes = downloadedIcons.Select(TryCreateWeatherConditionCode).ToArray();

            // Assert
            var weatherConditionCodesUnsuccessful = weatherConditionCodes
                .Where(c => !c.Success)
                .Select(c => c.Id)
                .ToArray();

            weatherConditionCodesUnsuccessful.Length.Should().Be(
                expected: 0,
                because: $"{nameof(WeatherConditionCode)}.{nameof(WeatherConditionCode.FromValue)} failed for icons with Id: {Environment.NewLine}" +
                $"[{string.Join(", ", weatherConditionCodesUnsuccessful)}]");
        }

        [Fact]
        public void ShouldReturnToString()
        {
            // Arrange
            var cultureInfo = new CultureInfo("de");
            var all = WeatherConditionCode.All.ToArray();

            // Act
            var weatherConditionCodeStrings = all.Select(c => (Code: c.Value, StringValue: c.ToString("G", cultureInfo))).ToArray();

            // Assert
            weatherConditionCodeStrings.Should().HaveCount(all.Length);
            weatherConditionCodeStrings.Where(s => s.StringValue == null).Should().BeEmpty();
        }
    }
}