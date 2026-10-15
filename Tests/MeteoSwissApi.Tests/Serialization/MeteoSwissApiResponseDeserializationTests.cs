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
        public void Deserialize_PlzDetailV3Strict_ReturnsWeatherInfo()
        {
            // Arrange
            var json = Fixtures.GetString("plzDetail_v3_633000.json");

            // Act
            var weatherInfo = JsonSerializer.Deserialize<WeatherInfo>(json, JsonSerialization.Strict);

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
        public void Deserialize_ForecastV3Strict_ReturnsForecastInfo()
        {
            // Arrange
            var json = Fixtures.GetString("forecast_v3_633000.json");

            // Act
            var forecastInfo = JsonSerializer.Deserialize<ForecastInfo>(json, JsonSerialization.Strict);

            // Assert
            forecastInfo.Should().NotBeNull();
            forecastInfo.Plz.Should().Be(633000);
            forecastInfo.CurrentWeather.Should().NotBeNull();
            forecastInfo.Forecast.Should().NotBeNullOrEmpty();
            forecastInfo.Graph.Should().NotBeNull();
        }

        [Fact]
        public void Deserialize_ForecastV3WithNullWarningsOverview_ReturnsEmptyArray()
        {
            // Arrange
            // The forecast endpoint returns "warningsOverview": null instead of an empty array.
            var json = Fixtures.GetString("forecast_v3_633000.json");

            // Act
            var forecastInfo = JsonSerializer.Deserialize<ForecastInfo>(json, JsonSerialization.Default);

            // Assert
            forecastInfo.Should().NotBeNull();
            forecastInfo.WarningsOverview.Should().BeEmpty();
        }

        [Fact]
        public void Deserialize_PlzDetailV3WithUnknownWarnTypeAndLevel_KeepsWarnings()
        {
            // Arrange
            // MeteoSwiss may add warn types and levels that this version does not know yet.
            var json = Fixtures.GetString("plzDetail_v3_633000.json")
                .Replace("\"warnType\":10,\"warnLevel\":4", "\"warnType\":99,\"warnLevel\":6");

            // Act
            var weatherInfo = JsonSerializer.Deserialize<WeatherInfo>(json, JsonSerialization.Default);

            // Assert
            weatherInfo.Should().NotBeNull();
            weatherInfo.Warnings.Select(w => (w.WarnType.Value, w.WarnLevel.Level)).Should().Contain((99, 6));
            weatherInfo.WarningsOverview.Select(w => (w.WarnType.Value, w.WarnLevel.Level)).Should().Contain((99, 6));
        }

        [Fact]
        public void Deserialize_PlzDetailV3_ReturnsUtcTimestamps()
        {
            // Arrange
            var json = Fixtures.GetString("plzDetail_v3_633000.json");

            // Act
            var weatherInfo = JsonSerializer.Deserialize<WeatherInfo>(json, JsonSerialization.Default);

            // Assert
            weatherInfo.Should().NotBeNull();
            weatherInfo.CurrentWeather.Time.Should().Be(new DateTime(2026, 7, 19, 16, 10, 0, DateTimeKind.Utc));
            weatherInfo.CurrentWeather.Time.Kind.Should().Be(DateTimeKind.Utc);
            weatherInfo.Graph.Start.Kind.Should().Be(DateTimeKind.Utc);
            weatherInfo.Graph.Sunrise.Should().OnlyContain(d => d.Kind == DateTimeKind.Utc);
            weatherInfo.Warnings.Should().OnlyContain(w => w.ValidFrom.Kind == DateTimeKind.Utc);
        }

        [Fact]
        public void Deserialize_PlzDetailV3_ReturnsForecastDayAsCalendarDate()
        {
            // Arrange
            var json = Fixtures.GetString("plzDetail_v3_633000.json");

            // Act
            var weatherInfo = JsonSerializer.Deserialize<WeatherInfo>(json, JsonSerialization.Default);

            // Assert
            weatherInfo.Should().NotBeNull();
            var dayDate = weatherInfo.Forecast[0].DayDate;
            dayDate.Should().Be(new DateTime(2026, 7, 19));
            dayDate.Kind.Should().Be(DateTimeKind.Unspecified);
        }
    }
}
