using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using MeteoSwissApi.Models;
using MeteoSwissApi.Models.Converters;
using MeteoSwissApi.Utils;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace MeteoSwissApi
{
    public class SwissMetNetService : ISwissMetNetService
    {
        private static readonly Uri ApiEndpoint = new Uri("https://data.geo.admin.ch", UriKind.Absolute);

        private readonly ILogger<SwissMetNetService> logger;
        private readonly MeteoSwissApiOptions options;
        private readonly HttpClient httpClient;
        private readonly JsonSerializerSettings serializerSettings;
        private readonly IMemoryCache memoryCache;

        private static readonly Encoding Windows1252Encoding = Encoding.GetEncoding("Windows-1252");

        private const string WeatherStationsCacheKey = "weatherStations";
        private const string LatestMeasurementsCacheKey = "latestMeasurements";

        /// <summary>
        /// Initializes a new instance of the <see cref="SwissMetNetService"/> class.
        /// </summary>
        public SwissMetNetService()
            : this(new NullLogger<SwissMetNetService>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SwissMetNetService"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        public SwissMetNetService(
            ILogger<SwissMetNetService> logger)
            : this(logger, new MeteoSwissApiOptions())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SwissMetNetService"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        public SwissMetNetService(
            IOptions<MeteoSwissApiOptions> options)
          : this(options.Value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SwissMetNetService"/> class.
        /// </summary>
        /// <param name="options">The service options.</param>
        public SwissMetNetService(
            MeteoSwissApiOptions options)
          : this(new NullLogger<SwissMetNetService>(), options)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SwissMetNetService"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="options">The service options.</param>
        public SwissMetNetService(
            ILogger<SwissMetNetService> logger,
            IOptions<MeteoSwissApiOptions> options)
          : this(logger, options.Value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SwissMetNetService"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="options">The service options.</param>
        public SwissMetNetService(
            ILogger<SwissMetNetService> logger,
            MeteoSwissApiOptions options)
          : this(logger, new HttpClient(), options)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SwissMetNetService"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="httpClient">The HttpClient instance.</param>
        /// <param name="options">The service options.</param>
        public SwissMetNetService(
            ILogger<SwissMetNetService> logger,
            HttpClient httpClient,
            MeteoSwissApiOptions options)
        {
            this.logger = logger;
            this.options = options;
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

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<WeatherStation>> GetWeatherStationsAsync(TimeSpan? cacheExpiration = null)
        {
            if (this.memoryCache.TryGetValue<IEnumerable<WeatherStation>>(WeatherStationsCacheKey, out var cache))
            {
                this.logger.LogDebug($"GetWeatherStationsAsync (from cache)");
                return cache;
            }
            else
            {
                this.logger.LogDebug($"GetWeatherStationsAsync");
            }

            var builder = new UriBuilder(ApiEndpoint)
            {
                Path = "ch.meteoschweiz.messnetz-automatisch/ch.meteoschweiz.messnetz-automatisch_en.csv",
            };

            var uri = builder.ToString();
            this.logger.LogDebug($"GetWeatherStationsAsync: GET {uri}");

            var response = await this.httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();

            var contentBytes = await response.Content.ReadAsByteArrayAsync();
            var csvContent = Windows1252Encoding.GetString(contentBytes, 0, contentBytes.Length);

            if (this.options.VerboseLogging)
            {
                this.logger.LogDebug($"GetWeatherStationsAsync returned content:{Environment.NewLine}{csvContent}");
            }

            var weatherStations = CsvImporter.Import<WeatherStation>(csvContent);

            cacheExpiration ??= this.options.SwissMetNet.CacheExpiration;
            if (cacheExpiration is TimeSpan cacheExpirationTimeSpan && weatherStations.Any())
            {
                this.memoryCache.Set(WeatherStationsCacheKey, weatherStations, cacheExpirationTimeSpan);
            }
            else
            {
                this.memoryCache.Remove(WeatherStationsCacheKey);
            }

            return weatherStations;
        }

        public async Task<WeatherStation> GetWeatherStationAsync(string stationCode, TimeSpan? cacheExpiration = null)
        {
            if (stationCode == null)
            {
                throw new ArgumentNullException(nameof(stationCode));
            }

            var weatherStation = (await this.GetWeatherStationsAsync(cacheExpiration))
                .Where(m => string.Equals(m.StationCode, stationCode, StringComparison.InvariantCultureIgnoreCase))
                .FirstOrDefault();

            return weatherStation;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<WeatherStationMeasurement>> GetLatestMeasurementsAsync(TimeSpan? cacheExpiration = null)
        {
            if (this.memoryCache.TryGetValue<IEnumerable<WeatherStationMeasurement>>(LatestMeasurementsCacheKey, out var measurementsCache))
            {
                this.logger.LogDebug($"GetLatestMeasurementsAsync (from cache)");
                return measurementsCache;
            }
            else
            {
                this.logger.LogDebug($"GetLatestMeasurementsAsync");
            }

            var builder = new UriBuilder(ApiEndpoint)
            {
                Path = "ch.meteoschweiz.messwerte-aktuell/VQHA80.csv",
            };

            var uri = builder.ToString();
            this.logger.LogDebug($"GetLatestMeasurementsAsync: GET {uri}");

            var response = await this.httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();

            var contentBytes = await response.Content.ReadAsByteArrayAsync();
            var csvContent = Windows1252Encoding.GetString(contentBytes, 0, contentBytes.Length);

            if (this.options.VerboseLogging)
            {
                this.logger.LogDebug($"GetLatestMeasurementsAsync returned content:{Environment.NewLine}{csvContent}");
            }

            var measurements = CsvImporter.Import<WeatherStationMeasurement>(csvContent);

            cacheExpiration ??= this.options.SwissMetNet.CacheExpiration;
            if (cacheExpiration is TimeSpan cacheExpirationTimeSpan && measurements.Any())
            {
                var measurementDate = measurements
                    .Select(m => m.Date)
                    .OrderBy(d => d)
                    .FirstOrDefault();

                var cacheExpirationDate = measurementDate + cacheExpirationTimeSpan;
                this.memoryCache.Set(LatestMeasurementsCacheKey, measurements, cacheExpirationDate);
            }
            else
            {
                this.memoryCache.Remove(LatestMeasurementsCacheKey);
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
