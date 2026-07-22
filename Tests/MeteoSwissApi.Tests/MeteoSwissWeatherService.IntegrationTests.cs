namespace MeteoSwissApi.Tests
{
    [Trait(Traits.Category, Traits.IntegrationTests)]
    public class MeteoSwissWeatherServiceIntegrationTests
    {
        private const string IconFileExtension = "svg";

        private readonly ILogger<MeteoSwissWeatherService> logger;
        private readonly MeteoSwissApiOptions options;
        private readonly ITestOutputHelper testOutputHelper;
        private readonly DumpOptions dumpOptions;
        private readonly TestHelper testHelper;

        public MeteoSwissWeatherServiceIntegrationTests(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
            this.logger = new TestOutputHelperLogger<MeteoSwissWeatherService>(testOutputHelper);
            this.options = new MeteoSwissApiOptions
            {
                VerboseLogging = true
            };

            this.dumpOptions = new DumpOptions
            {
                DumpStyle = DumpStyle.CSharp,
                SetPropertiesOnly = true
            };

            this.dumpOptions.CustomInstanceFormatters.AddFormatter<Temperature>(t => $"Temperature.FromDegreesCelsius({t.Value})");

            this.testHelper = new TestHelper(testOutputHelper);
        }

        [Theory]
        [InlineData(6330)]
        [InlineData(633000)]
        [InlineData(690000)]
        [InlineData(601000)]
        [InlineData(601002)]
        [InlineData(671700)]
        [InlineData(195000)]
        [InlineData(774200)]
        public async Task ShouldGetCurrentWeatherAsync(int plz)
        {
            // Arrange
            var meteoSwissWeatherService = new MeteoSwissWeatherService(this.logger, this.options);
            meteoSwissWeatherService.ThrowExceptionOnMissingJsonProperties = true;

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
        [InlineData(6330)]
        [InlineData(633000)]
        [InlineData(690000)]
        [InlineData(601000)]
        [InlineData(601002)]
        [InlineData(671700)]
        [InlineData(195000)]
        [InlineData(774200)]
        public async Task ShouldGetForecastAsync(int plz)
        {
            // Arrange
            var meteoSwissWeatherService = new MeteoSwissWeatherService(this.logger, this.options);
            meteoSwissWeatherService.ThrowExceptionOnMissingJsonProperties = true;

            // Act
            var forecastInfo = await meteoSwissWeatherService.GetForecastAsync(plz);

            // Assert
            this.testOutputHelper.WriteLine(ObjectDumper.Dump(forecastInfo, this.dumpOptions));

            forecastInfo.Should().NotBeNull();
        }

        [Theory]
        [InlineData("en")]
        [InlineData("de")]
        [InlineData("fr")]
        [InlineData("it")]
        public async Task ShouldGetCurrentWeatherAsync_WithLanguage(string language)
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
        public async Task ShouldGetWeatherIconAsync_6330()
        {
            // Arrange
            const int plz = 6330;

            IMeteoSwissWeatherService meteoSwissWeatherService = new MeteoSwissWeatherService(this.logger, this.options);
            var weatherInfo = await meteoSwissWeatherService.GetCurrentWeatherAsync(plz);

            // Act
            var iconStream = await meteoSwissWeatherService.GetWeatherIconAsync(weatherInfo.CurrentWeather.IconV2);

            // Assert
            iconStream.Should().NotBeNull();
            this.testHelper.WriteFile(iconStream, fileExtension: IconFileExtension);
        }

        [Theory]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        public async Task ShouldGetWarningIconAsync(int warnLevel)
        {
            // Arrange
            IMeteoSwissWeatherService meteoSwissWeatherService = new MeteoSwissWeatherService(this.logger, this.options);

            // Act
            var iconStream = await meteoSwissWeatherService.GetWarningIconAsync(warnLevel);

            // Assert
            iconStream.Should().NotBeNull();
            this.testHelper.WriteFile(iconStream, fileName: $"{nameof(this.ShouldGetWarningIconAsync)}_{warnLevel}", fileExtension: IconFileExtension);
        }

        [Fact]
        public async Task ShouldGetWarningIconAsync_ReturnsTransparentIcon_IfWarnLevelHasNoIcon()
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