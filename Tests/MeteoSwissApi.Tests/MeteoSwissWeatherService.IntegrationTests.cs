namespace MeteoSwissApi.Tests
{
    /// <summary>
    /// The tests in this file call the live MeteoSwiss services.
    /// </summary>
    [Collection(Collections.IntegrationTests)]
    public partial class MeteoSwissWeatherServiceTests
    {
        private const string IconFileExtension = "svg";

        private readonly MeteoSwissApiOptions options;
        private readonly ITestOutputHelper testOutputHelper;
        private readonly DumpOptions dumpOptions;
        private readonly TestHelper testHelper;

        [Theory]
        [Trait(Traits.Category, Traits.IntegrationTests)]
        [InlineData(6330)]
        [InlineData(633000)]
        [InlineData(690000)]
        [InlineData(601000)]
        [InlineData(601002)]
        [InlineData(671700)]
        [InlineData(195000)]
        [InlineData(774200)]
        public async Task GetCurrentWeatherAsync_Live_ReturnsWeatherInfo(int plz)
        {
            // Arrange
            var meteoSwissWeatherService = new MeteoSwissWeatherService(this.logger, this.options);
            meteoSwissWeatherService.DisallowUnmappedJsonMembers = true;

            // Act
            var weatherInfo = await meteoSwissWeatherService.GetCurrentWeatherAsync(plz);

            // Assert
            this.testOutputHelper.WriteLine(ObjectDumper.Dump(weatherInfo, this.dumpOptions));

            weatherInfo.Should().NotBeNull();
            weatherInfo.CurrentWeather.Should().NotBeNull();

            if (weatherInfo.CurrentWeather.Temperature is Temperature temperature)
            {
                // Temperature is null when MeteoSwiss reports the 'no data' sentinel (32767);
                // otherwise it must be a physically plausible value.
                temperature.DegreesCelsius.Should().BeInRange(-100, 100);
            }
        }

        [Theory]
        [Trait(Traits.Category, Traits.IntegrationTests)]
        [InlineData(6330)]
        [InlineData(633000)]
        [InlineData(690000)]
        [InlineData(601000)]
        [InlineData(601002)]
        [InlineData(671700)]
        [InlineData(195000)]
        [InlineData(774200)]
        public async Task GetForecastAsync_Live_ReturnsForecastInfo(int plz)
        {
            // Arrange
            var meteoSwissWeatherService = new MeteoSwissWeatherService(this.logger, this.options);
            meteoSwissWeatherService.DisallowUnmappedJsonMembers = true;

            // Act
            var forecastInfo = await meteoSwissWeatherService.GetForecastAsync(plz);

            // Assert
            this.testOutputHelper.WriteLine(ObjectDumper.Dump(forecastInfo, this.dumpOptions));

            forecastInfo.Should().NotBeNull();
        }

        [Theory]
        [Trait(Traits.Category, Traits.IntegrationTests)]
        [InlineData("en")]
        [InlineData("de")]
        [InlineData("fr")]
        [InlineData("it")]
        public async Task GetCurrentWeatherAsync_LiveWithLanguage_ReturnsWeatherInfo(string language)
        {
            // Arrange
            const int plz = 774200;

            var options = new MeteoSwissApiOptions
            {
                Language = language,
                VerboseLogging = true
            };

            IMeteoSwissWeatherService meteoSwissWeatherService = new MeteoSwissWeatherService(this.logger, options);

            // Act
            var weatherInfo = await meteoSwissWeatherService.GetCurrentWeatherAsync(plz);

            // Assert
            this.testOutputHelper.WriteLine(ObjectDumper.Dump(weatherInfo, this.dumpOptions));

            weatherInfo.Should().NotBeNull();
        }

        [Fact]
        [Trait(Traits.Category, Traits.IntegrationTests)]
        public async Task ShouldGetWeatherIconAsync_6330()
        {
            // Arrange
            const int plz = 6330;

            IMeteoSwissWeatherService meteoSwissWeatherService = new MeteoSwissWeatherService(this.logger, this.options);
            var weatherInfo = await meteoSwissWeatherService.GetCurrentWeatherAsync(plz);
            var iconId = weatherInfo.CurrentWeather.IconV2!.Value;

            // Act
            var iconStream = await meteoSwissWeatherService.GetWeatherIconAsync(iconId);

            // Assert
            iconStream.Should().NotBeNull();
            this.testHelper.WriteFile(iconStream, fileExtension: IconFileExtension);
        }

        [Theory]
        [Trait(Traits.Category, Traits.IntegrationTests)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        public async Task GetWarningIconAsync_Live_ReturnsIcon(int warnLevel)
        {
            // Arrange
            IMeteoSwissWeatherService meteoSwissWeatherService = new MeteoSwissWeatherService(this.logger, this.options);

            // Act
            var iconStream = await meteoSwissWeatherService.GetWarningIconAsync(WarnLevel.FromValue(warnLevel));

            // Assert
            iconStream.Should().NotBeNull();
            this.testHelper.WriteFile(iconStream, fileName: $"{nameof(this.GetWarningIconAsync_Live_ReturnsIcon)}_{warnLevel}", fileExtension: IconFileExtension);
        }

        [Fact]
        [Trait(Traits.Category, Traits.IntegrationTests)]
        public async Task GetWarningIconAsync_LiveWarnLevelWithoutIcon_ReturnsTransparentIcon()
        {
            // Arrange
            IMeteoSwissWeatherService meteoSwissWeatherService = new MeteoSwissWeatherService(this.logger, this.options);

            // Act
            var iconStream = await meteoSwissWeatherService.GetWarningIconAsync(WarnLevel.Level1);

            // Assert
            iconStream.Should().NotBeNull();

            using var streamReader = new StreamReader(iconStream);
            var content = await streamReader.ReadToEndAsync();
            content.Should().Contain("width=\"1px\"");
            content.Should().Contain("height=\"1px\"");
        }
    }
}