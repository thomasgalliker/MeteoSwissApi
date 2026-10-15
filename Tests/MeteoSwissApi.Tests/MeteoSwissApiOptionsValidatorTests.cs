namespace MeteoSwissApi.Tests
{
    [Trait(Traits.Category, Traits.UnitTests)]
    public class MeteoSwissApiOptionsValidatorTests
    {
        [Fact]
        public void Validate_DefaultOptions_Succeeds()
        {
            // Arrange
            var validator = new MeteoSwissApiOptionsValidator();

            // Act
            var result = validator.Validate(null, new MeteoSwissApiOptions());

            // Assert
            result.Succeeded.Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("es")]
        public void Validate_UnsupportedLanguage_Fails(string? language)
        {
            // Arrange
            var validator = new MeteoSwissApiOptionsValidator();
            var options = new MeteoSwissApiOptions { Language = language! };

            // Act
            var result = validator.Validate(null, options);

            // Assert
            result.Failed.Should().BeTrue();
            result.FailureMessage.Should().Contain("Language must be one of: de, fr, it, en.");
        }

        [Fact]
        public void Validate_NonPositiveCacheExpiration_Fails()
        {
            // Arrange
            var validator = new MeteoSwissApiOptionsValidator();
            var options = new MeteoSwissApiOptions { SwissMetNet = { CacheExpiration = TimeSpan.Zero } };

            // Act
            var result = validator.Validate(null, options);

            // Assert
            result.Failed.Should().BeTrue();
            result.FailureMessage.Should().Contain("CacheExpiration must be positive");
        }

        [Fact]
        public void Validate_UnsupportedLanguageAndNullSwissMetNetOptions_ReportsBothErrors()
        {
            // Arrange
            var validator = new MeteoSwissApiOptionsValidator();
            var options = new MeteoSwissApiOptions { Language = "es", SwissMetNet = null! };

            // Act
            var result = validator.Validate(null, options);

            // Assert
            result.Failures.Should().HaveCount(2);
        }

        [Fact]
        public void SupportedLanguages_Always_AreTheMeteoSwissLanguages()
        {
            // Act
            var supportedLanguages = MeteoSwissApiOptions.SupportedLanguages;

            // Assert
            supportedLanguages.Should().Equal("de", "fr", "it", "en");
        }
        [Fact]
        public void ThrowIfInvalid_OptionsIsNull_ThrowsArgumentNullException()
        {
            // Act
            Action action = () => MeteoSwissApiOptionsValidator.ThrowIfInvalid(null!, "options");

            // Assert
            action.Should().Throw<ArgumentNullException>().WithParameterName("options");
        }
    }
}
