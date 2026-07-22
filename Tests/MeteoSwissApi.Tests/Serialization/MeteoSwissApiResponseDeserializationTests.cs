using System.Text.Json;
using MeteoSwissApi.Serialization;

namespace MeteoSwissApi.Tests.Serialization
{
    /// <summary>
    /// Offline regression tests that deserialize captured MeteoSwiss <c>v3</c> API responses
    /// with strict options (<see cref="System.Text.Json.Serialization.JsonUnmappedMemberHandling.Disallow"/>).
    /// If the live API adds a property the models don't declare — or a fixture is refreshed with a
    /// new shape — deserialization throws and these tests fail, catching model/response drift
    /// without hitting the network. The fixtures live in <c>Resources/ApiResponses</c>.
    /// </summary>
    [Trait(Traits.Category, Traits.UnitTests)]
    public class MeteoSwissApiResponseDeserializationTests
    {
        [Fact]
        public void ShouldDeserializePlzDetailV3_Strict()
        {
            // Arrange
            var json = LoadFixture("plzDetail_v3_633000.json");

            // Act
            var weatherInfo = JsonSerializer.Deserialize<WeatherInfo>(json, JsonSerialization.CreateOptions(disallowUnmappedMembers: true));

            // Assert
            weatherInfo.Should().NotBeNull();
            weatherInfo.CurrentWeather.Should().NotBeNull();
            weatherInfo.CurrentWeather.Temperature.Should().NotBeNull();
            weatherInfo.Forecast.Should().NotBeNullOrEmpty();
            weatherInfo.Warnings.Should().NotBeNull();
            weatherInfo.WarningsOverview.Should().NotBeNull();
            weatherInfo.Graph.Should().NotBeNull();
            weatherInfo.KlimaGraph.Should().NotBeNull();
        }

        [Fact]
        public void ShouldDeserializeForecastV3_Strict()
        {
            // Arrange
            var json = LoadFixture("forecast_v3_633000.json");

            // Act
            var forecastInfo = JsonSerializer.Deserialize<ForecastInfo>(json, JsonSerialization.CreateOptions(disallowUnmappedMembers: true));

            // Assert
            forecastInfo.Should().NotBeNull();
            forecastInfo.Plz.Should().Be(633000);
            forecastInfo.CurrentWeather.Should().NotBeNull();
            forecastInfo.Forecast.Should().NotBeNullOrEmpty();
            forecastInfo.Graph.Should().NotBeNull();
            // Note: the forecast endpoint returns "warningsOverview": null (not an empty array),
            // so WarningsOverview is intentionally not asserted to be non-null here.
        }

        private static string LoadFixture(string fileName)
        {
            var assembly = typeof(MeteoSwissApiResponseDeserializationTests).Assembly;
            var resourceName = assembly.GetManifestResourceNames()
                .Single(n => n.EndsWith(fileName, StringComparison.Ordinal));

            using var stream = assembly.GetManifestResourceStream(resourceName)!;
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}
