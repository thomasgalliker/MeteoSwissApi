using System.Net.Http.Headers;
using CsvHelper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace MeteoSwissApi
{
    public sealed class SwissMetNetService : ISwissMetNetService, IDisposable
    {
        private static readonly Uri ApiEndpoint = new Uri("https://data.geo.admin.ch", UriKind.Absolute);

        private const string WeatherStationsPath = "ch.meteoschweiz.messnetz-automatisch/ch.meteoschweiz.messnetz-automatisch_en.csv";
        private const string LatestMeasurementsPath = "ch.meteoschweiz.messwerte-aktuell/VQHA80.csv";

        private const string WeatherStationsCacheKey = "weatherStations";
        private const string LatestMeasurementsCacheKey = "latestMeasurements";

        private const int Windows1252CodePage = 1252;

        private static readonly Encoding Windows1252Encoding;

        // The files change every ten minutes, so no intermediate cache may serve an old copy. The header value is created per
        // request: HttpHeaders parses a later Cache-Control value of the same request into the stored instance, so a shared
        // instance could be changed for every later request by a message handler of the caller.
        private static readonly Action<HttpRequestHeaders> ConfigureHeaders = headers => headers.CacheControl = new CacheControlHeaderValue { NoCache = true };

        private readonly ILogger<SwissMetNetService> logger;
        private readonly TimeSpan? cacheExpiration;
        private readonly bool verboseLogging;
        private readonly HttpClient httpClient;
        private readonly bool disposeHttpClient;
        private readonly MemoryCache memoryCache;

        // One download per file at a time: callers that miss the cache at the same time wait for the running download.
        private readonly SemaphoreSlim weatherStationsLock = new SemaphoreSlim(1, 1);
        private readonly SemaphoreSlim latestMeasurementsLock = new SemaphoreSlim(1, 1);

        static SwissMetNetService()
        {
            // The CSV files are encoded in Windows-1252, which .NET Core only knows once the code pages provider is registered.
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Windows1252Encoding = Encoding.GetEncoding(Windows1252CodePage);
        }

        /// <summary>
        /// Creates the service with the default options, its own <see cref="HttpClient"/> and no logging.
        /// </summary>
        public SwissMetNetService()
            : this(NullLogger<SwissMetNetService>.Instance)
        {
        }

        /// <summary>
        /// Creates the service with the default options and its own <see cref="HttpClient"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="logger"/> is null.</exception>
        public SwissMetNetService(
            ILogger<SwissMetNetService> logger)
            : this(logger, new MeteoSwissApiOptions())
        {
        }

        /// <summary>
        /// Creates the service with options from dependency injection, its own <see cref="HttpClient"/> and no logging.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="options"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="options"/> is invalid, for example its cache expiration is not positive.</exception>
        public SwissMetNetService(
            IOptions<MeteoSwissApiOptions> options)
          : this(NullLogger<SwissMetNetService>.Instance, options)
        {
        }

        /// <summary>
        /// Creates the service with the given options, its own <see cref="HttpClient"/> and no logging.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="options"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="options"/> is invalid, for example its cache expiration is not positive.</exception>
        public SwissMetNetService(
            MeteoSwissApiOptions options)
          : this(NullLogger<SwissMetNetService>.Instance, options)
        {
        }

        /// <summary>
        /// Creates the service with options from dependency injection and its own <see cref="HttpClient"/>.
        /// This is the constructor that <c>AddMeteoSwissApi</c> uses.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="logger"/> or <paramref name="options"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="options"/> is invalid, for example its cache expiration is not positive.</exception>
        public SwissMetNetService(
            ILogger<SwissMetNetService> logger,
            IOptions<MeteoSwissApiOptions> options)
          : this(logger, (options ?? throw new ArgumentNullException(nameof(options))).Value)
        {
        }

        /// <summary>
        /// Creates the service with the given options and its own <see cref="HttpClient"/>.
        /// </summary>
        /// <remarks>
        /// The client keeps its connections for the lifetime of the service and uses the default timeout of <see cref="HttpClient"/>,
        /// 100 seconds. Create the service once and reuse it, and dispose it to release the client and the cache.
        /// Pass your own client for a different timeout or a retry policy.
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="logger"/> or <paramref name="options"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="options"/> is invalid, for example its cache expiration is not positive.</exception>
        public SwissMetNetService(
            ILogger<SwissMetNetService> logger,
            MeteoSwissApiOptions options)
          : this(logger, options, null)
        {
        }

        /// <summary>
        /// Creates the service with the given <see cref="HttpClient"/>, for example one with a shorter timeout or with your own message handlers.
        /// The service sets its headers per request, never changes the client and does not dispose it.
        /// </summary>
        /// <inheritdoc cref="MeteoSwissWeatherService(ILogger{MeteoSwissWeatherService}, HttpClient, MeteoSwissApiOptions)" path="/remarks"/>
        /// <exception cref="ArgumentNullException"><paramref name="logger"/>, <paramref name="httpClient"/> or <paramref name="options"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="options"/> is invalid, for example its cache expiration is not positive.</exception>
        public SwissMetNetService(
            ILogger<SwissMetNetService> logger,
            HttpClient httpClient,
            MeteoSwissApiOptions options)
          : this(logger, options, httpClient ?? throw new ArgumentNullException(nameof(httpClient)))
        {
        }

        /// <summary>
        /// Creates the service with <paramref name="httpClient"/>, or with its own client, which it disposes, if <paramref name="httpClient"/> is null.
        /// </summary>
        private SwissMetNetService(
            ILogger<SwissMetNetService> logger,
            MeteoSwissApiOptions options,
            HttpClient? httpClient)
        {
            MeteoSwissApiOptionsValidator.ThrowIfInvalid(options, nameof(options));

            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.disposeHttpClient = httpClient is null;
            this.httpClient = httpClient ?? new HttpClient();
            this.memoryCache = new MemoryCache(new MemoryCacheOptions());

            // The validated values are copied, as in the other services, so that a later change to the options object
            // cannot reach the service unvalidated.
            this.cacheExpiration = options.SwissMetNet.CacheExpiration;
            this.verboseLogging = options.VerboseLogging;
        }

        public Task<IReadOnlyList<WeatherStation>> GetWeatherStationsAsync(CancellationToken cancellationToken = default)
        {
            return this.GetWeatherStationsAsync(null, cancellationToken);
        }

        public async Task<IReadOnlyList<WeatherStation>> GetWeatherStationsAsync(TimeSpan? cacheExpiration, CancellationToken cancellationToken = default)
        {
            cacheExpiration = this.GetCacheExpiration(cacheExpiration);

            var weatherStations = this.GetFromCache<WeatherStation>(WeatherStationsCacheKey, nameof(this.GetWeatherStationsAsync));
            if (weatherStations != null)
            {
                return weatherStations;
            }

            await this.weatherStationsLock.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                // Another caller may have cached the stations while this one waited.
                weatherStations = this.GetFromCache<WeatherStation>(WeatherStationsCacheKey, nameof(this.GetWeatherStationsAsync));
                if (weatherStations != null)
                {
                    return weatherStations;
                }

                var uri = new Uri(ApiEndpoint, WeatherStationsPath);
                var rows = await this.GetCsvAsync<WeatherStation>(nameof(this.GetWeatherStationsAsync), uri, cancellationToken).ConfigureAwait(false);
                weatherStations = Array.AsReadOnly(rows);

                // The stations change rarely, so they expire a fixed time after the download.
                var expiresAt = cacheExpiration is TimeSpan expiration ? GetExpiresAt(DateTimeOffset.UtcNow, expiration) : (DateTimeOffset?)null;
                this.SetCache(WeatherStationsCacheKey, weatherStations, expiresAt);

                return weatherStations;
            }
            finally
            {
                this.weatherStationsLock.Release();
            }
        }

        public Task<WeatherStation?> GetWeatherStationAsync(string stationCode, CancellationToken cancellationToken = default)
        {
            return this.GetWeatherStationAsync(stationCode, null, cancellationToken);
        }

        public async Task<WeatherStation?> GetWeatherStationAsync(string stationCode, TimeSpan? cacheExpiration, CancellationToken cancellationToken = default)
        {
            if (stationCode == null)
            {
                throw new ArgumentNullException(nameof(stationCode));
            }

            var weatherStations = await this.GetWeatherStationsAsync(cacheExpiration, cancellationToken).ConfigureAwait(false);
            return weatherStations.FirstOrDefault(s => string.Equals(s.StationCode, stationCode, StringComparison.OrdinalIgnoreCase));
        }

        public Task<IReadOnlyList<WeatherStationMeasurement>> GetLatestMeasurementsAsync(CancellationToken cancellationToken = default)
        {
            return this.GetLatestMeasurementsAsync(null, cancellationToken);
        }

        public async Task<IReadOnlyList<WeatherStationMeasurement>> GetLatestMeasurementsAsync(TimeSpan? cacheExpiration, CancellationToken cancellationToken = default)
        {
            cacheExpiration = this.GetCacheExpiration(cacheExpiration);

            var measurements = this.GetFromCache<WeatherStationMeasurement>(LatestMeasurementsCacheKey, nameof(this.GetLatestMeasurementsAsync));
            if (measurements != null)
            {
                return measurements;
            }

            await this.latestMeasurementsLock.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                // Another caller may have cached the measurements while this one waited.
                measurements = this.GetFromCache<WeatherStationMeasurement>(LatestMeasurementsCacheKey, nameof(this.GetLatestMeasurementsAsync));
                if (measurements != null)
                {
                    return measurements;
                }

                var uri = new Uri(ApiEndpoint, LatestMeasurementsPath);
                var rows = await this.GetCsvAsync<WeatherStationMeasurement>(nameof(this.GetLatestMeasurementsAsync), uri, cancellationToken).ConfigureAwait(false);

                // A row without a readable date cannot be placed in time, so it is skipped.
                var rowsWithDate = rows.Where(m => m.Date != DateTime.MinValue).ToArray();
                if (rowsWithDate.Length < rows.Length)
                {
                    this.logger.LogDebug("GetLatestMeasurementsAsync: skipped {Count} rows without a readable date", rows.Length - rowsWithDate.Length);
                }

                measurements = Array.AsReadOnly(rowsWithDate);

                // MeteoSwiss publishes new values every 10 minutes. The measurements are cached until the newest one
                // is cacheExpiration old, so that the cache does not hold back a newer publication for longer than that.
                var expiresAt = (DateTimeOffset?)null;
                if (cacheExpiration is TimeSpan expiration && measurements.Count > 0)
                {
                    var newestMeasurementDate = measurements.Max(m => m.Date);
                    expiresAt = GetExpiresAt(new DateTimeOffset(newestMeasurementDate, TimeSpan.Zero), expiration);
                }

                this.SetCache(LatestMeasurementsCacheKey, measurements, expiresAt);

                return measurements;
            }
            finally
            {
                this.latestMeasurementsLock.Release();
            }
        }

        public Task<WeatherStationMeasurement?> GetLatestMeasurementAsync(string stationCode, CancellationToken cancellationToken = default)
        {
            return this.GetLatestMeasurementAsync(stationCode, null, cancellationToken);
        }

        public async Task<WeatherStationMeasurement?> GetLatestMeasurementAsync(string stationCode, TimeSpan? cacheExpiration, CancellationToken cancellationToken = default)
        {
            if (stationCode == null)
            {
                throw new ArgumentNullException(nameof(stationCode));
            }

            var measurements = await this.GetLatestMeasurementsAsync(cacheExpiration, cancellationToken).ConfigureAwait(false);
            return measurements.FirstOrDefault(m => string.Equals(m.StationCode, stationCode, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Releases the cache, the locks and the <see cref="HttpClient"/> that the service created itself. A client passed to the
        /// constructor is not disposed: it belongs to the caller.
        /// Dispose the service after its last call has completed: a call that is still waiting for a download fails once the locks are disposed.
        /// </summary>
        public void Dispose()
        {
            this.memoryCache.Dispose();
            this.weatherStationsLock.Dispose();
            this.latestMeasurementsLock.Dispose();

            if (this.disposeHttpClient)
            {
                this.httpClient.Dispose();
            }
        }

        /// <summary>
        /// Returns <paramref name="cacheExpiration"/>, or the configured expiration if it is <c>null</c>.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="cacheExpiration"/> is zero or negative.</exception>
        private TimeSpan? GetCacheExpiration(TimeSpan? cacheExpiration)
        {
            if (cacheExpiration is TimeSpan expiration && expiration <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(cacheExpiration), cacheExpiration, "The cache expiration must be positive, or null to use the configured expiration.");
            }

            return cacheExpiration ?? this.cacheExpiration;
        }

        /// <summary>
        /// Returns <paramref name="start"/> plus <paramref name="expiration"/>, or <see cref="DateTimeOffset.MaxValue"/> if the sum
        /// is out of range, so that a very long expiration such as <see cref="TimeSpan.MaxValue"/> caches for good instead of failing.
        /// </summary>
        private static DateTimeOffset GetExpiresAt(DateTimeOffset start, TimeSpan expiration)
        {
            return expiration > DateTimeOffset.MaxValue - start ? DateTimeOffset.MaxValue : start + expiration;
        }

        private IReadOnlyList<T>? GetFromCache<T>(string cacheKey, string methodName)
        {
            if (this.memoryCache.TryGetValue<IReadOnlyList<T>>(cacheKey, out var cachedValue) && cachedValue != null)
            {
                this.logger.LogDebug("{MethodName}: returned from cache", methodName);
                return cachedValue;
            }

            return null;
        }

        /// <summary>
        /// Caches <paramref name="value"/> until <paramref name="expiresAt"/>, or drops the cached value if there is nothing to cache.
        /// </summary>
        private void SetCache<T>(string cacheKey, IReadOnlyList<T> value, DateTimeOffset? expiresAt)
        {
            if (expiresAt is DateTimeOffset absoluteExpiration && absoluteExpiration > DateTimeOffset.UtcNow && value.Count > 0)
            {
                this.memoryCache.Set(cacheKey, value, absoluteExpiration);
            }
            else
            {
                this.memoryCache.Remove(cacheKey);
            }
        }

        /// <summary>
        /// Downloads the Windows-1252 encoded CSV file at <paramref name="uri"/> and reads its records.
        /// </summary>
        /// <exception cref="MeteoSwissApiException">The request failed, or the response is empty or cannot be read as <typeparamref name="T"/>.</exception>
        private async Task<T[]> GetCsvAsync<T>(string methodName, Uri uri, CancellationToken cancellationToken)
        {
            this.logger.LogDebug("{MethodName}: GET {Uri}", methodName, uri);

            var contentBytes = await this.httpClient.GetBytesOrThrowAsync(uri, ConfigureHeaders, cancellationToken).ConfigureAwait(false);
            var csvContent = Windows1252Encoding.GetString(contentBytes, 0, contentBytes.Length);

            if (this.verboseLogging)
            {
                this.logger.LogDebug("{MethodName} returned content: {Content}", methodName, csvContent);
            }

            if (string.IsNullOrWhiteSpace(csvContent))
            {
                throw new MeteoSwissApiException($"GET {uri} returned an empty response.", uri, null, null);
            }

            try
            {
                return CsvImporter.Import<T>(csvContent, this.logger).ToArray();
            }
            catch (CsvHelperException ex)
            {
                throw new MeteoSwissApiException($"The response of GET {uri} could not be read as {typeof(T).Name}.", uri, null, ex);
            }
        }
    }
}
