using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using MeteoSwissApi.Models;
using MeteoSwissApi.Models.Converters;
using MeteoSwissApi.Utils;
using Microsoft.Extensions.Caching.Memory;
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
        private readonly IMemoryCache memoryCache;

        private const string MeasurementsCacheKey = "weatherStations";

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
            this.httpClient.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue
            {
                NoCache = true
            };

            this.serializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
            };

            this.serializerSettings.Converters.Add(new TemperatureJsonConverter());

            this.memoryCache = new MemoryCache(new MemoryCacheOptions { });
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

        public async Task<WeatherStation> GetWeatherStationAsync(string stationCode)
        {
            if (stationCode == null)
            {
                throw new ArgumentNullException(nameof(stationCode));
            }

            var weatherStation = (await this.GetWeatherStationsAsync())
                .Where(m => string.Equals(m.StationCode, stationCode, StringComparison.InvariantCultureIgnoreCase))
                .FirstOrDefault();

            return weatherStation;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<WeatherStationMeasurement>> GetLatestMeasurementsAsync(TimeSpan? cacheExpiration = null)
        {
            if (this.memoryCache.TryGetValue<IEnumerable<WeatherStationMeasurement>>(MeasurementsCacheKey, out var measurementsCache))
            {
                this.logger.LogDebug($"GetLatestMeasurementsAsync (from cache)");
                return measurementsCache;
            }
            else
            {
                this.logger.LogDebug($"GetLatestMeasurementsAsync");
            }

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

            if (cacheExpiration is TimeSpan cacheExpirationTimeSpan && measurements.Any())
            {
                var measurementDate = measurements
                    .Select(m => m.Date)
                    .OrderBy(d => d)
                    .FirstOrDefault();

                var cacheExpirationDate = measurementDate + cacheExpirationTimeSpan;
                this.memoryCache.Set(MeasurementsCacheKey, measurements, cacheExpirationDate);
            }
            else
            {
                this.memoryCache.Remove(MeasurementsCacheKey);
            }

            return measurements;
        }

        public async Task<WeatherStationMeasurement> GetLatestMeasurementAsync(string stationCode, TimeSpan? cacheExpiration = null)
        {
            if (stationCode == null)
            {
                throw new ArgumentNullException(nameof(stationCode));
            }

            var weatherStationMeasurement = (await this.GetLatestMeasurementsAsync(cacheExpiration))
                .Where(m => string.Equals(m.StationCode, stationCode, StringComparison.InvariantCultureIgnoreCase))
                .SingleOrDefault();

            return weatherStationMeasurement;
        }
    }
}
