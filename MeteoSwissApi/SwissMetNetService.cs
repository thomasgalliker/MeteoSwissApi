using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using MeteoSwissApi.Models;
using MeteoSwissApi.Models.Converters;
using MeteoSwissApi.Utils;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MeteoSwissApi
{
    public class SwissMetNetService : ISwissMetNetService
    {
        private readonly ILogger<SwissMetNetService> logger;
        private readonly Uri apiEndpoint;
        private readonly bool verboseLogging;
        private readonly HttpClient httpClient;
        private readonly JsonSerializerSettings serializerSettings;

        public SwissMetNetService(ILogger<SwissMetNetService> logger, ISwissMetNetServiceOptions options)
          : this(logger, new HttpClient(), options)
        {
        }

        public SwissMetNetService(ILogger<SwissMetNetService> logger, HttpClient httpClient, ISwissMetNetServiceOptions options)
        {
            this.logger = logger;
            this.apiEndpoint = new Uri(options.ApiEndpoint, UriKind.Absolute);
            this.verboseLogging = options.VerboseLogging;
            this.httpClient = httpClient;
            this.serializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
            };

            this.serializerSettings.Converters.Add(new TemperatureJsonConverter());
        }

        /// <inheritdoc />
        public async Task<IEnumerable<WeatherStation>> GetWeatherStationsAsync()
        {
            this.logger.LogDebug($"GetWeatherStationsAsync");

            var builder = new UriBuilder(this.apiEndpoint)
            {
                Path = "ch.meteoschweiz.messnetz-automatisch/ch.meteoschweiz.messnetz-automatisch_en.csv",
            };

            var uri = builder.ToString();
            this.logger.LogDebug($"GetWeatherStationsAsync: GET {uri}");

            var response = await this.httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();

            var csvContent = await response.Content.ReadAsStringAsync();

            if (this.verboseLogging)
            {
                this.logger.LogDebug($"GetWeatherStationsAsync returned content:{Environment.NewLine}{csvContent}");
            }

            var weatherStations = CsvImporter.Import<WeatherStation>(csvContent);
            return weatherStations;
        }

        public async Task<IEnumerable<WeatherStationMeasurement>> GetLatestMeasurementsAsync()
        {
            this.logger.LogDebug($"GetLatestMeasurementsAsync");

            var builder = new UriBuilder(this.apiEndpoint)
            {
                Path = "ch.meteoschweiz.messwerte-aktuell/VQHA80.csv",
            };

            var uri = builder.ToString();
            this.logger.LogDebug($"GetLatestMeasurementsAsync: GET {uri}");

            var response = await this.httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();

            var csvContent = await response.Content.ReadAsStringAsync();

            if (this.verboseLogging)
            {
                this.logger.LogDebug($"GetLatestMeasurementsAsync returned content:{Environment.NewLine}{csvContent}");
            }

            var measurements = CsvImporter.Import<WeatherStationMeasurement>(csvContent);
            return measurements;
        }
    }
}
