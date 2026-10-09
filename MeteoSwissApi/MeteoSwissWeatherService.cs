using System.Net.Http.Headers;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace MeteoSwissApi
{
    public sealed class MeteoSwissWeatherService : IMeteoSwissWeatherService, IDisposable
    {
        private static readonly Uri ApiEndpoint = new Uri("https://app-prod-ws.meteoswiss-app.ch", UriKind.Absolute);

        private const string ApiVersion = "v3";

        private const int MinPlz = 1000;
        private const int MaxPlz = 9999;
        private const int MinPlzWithSuffix = 100000;
        private const int MaxPlzWithSuffix = 999999;
        private const string DefaultPlzSuffix = "00";

        /// <summary>
        /// The forecast endpoint answers an unknown postal code with a placeholder whose plz is 0.
        /// </summary>
        private const int UnknownPlz = 0;

        private readonly ILogger logger;
        private readonly HttpClient httpClient;
        private readonly bool disposeHttpClient;
        private readonly Action<HttpRequestHeaders> configureHeaders;
        private readonly IWeatherIconMapping defaultWeatherIconMapping;
        private readonly IWarningIconMapping defaultWarningIconMapping;
        private readonly bool verboseLogging;

        /// <summary>
        /// Creates the service with the default options, its own <see cref="HttpClient"/> and no logging.
        /// </summary>
        public MeteoSwissWeatherService()
            : this(NullLogger<MeteoSwissWeatherService>.Instance)
        {
        }

        /// <summary>
        /// Creates the service with the default options and its own <see cref="HttpClient"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="logger"/> is null.</exception>
        public MeteoSwissWeatherService(
            ILogger<MeteoSwissWeatherService> logger)
            : this(logger, new MeteoSwissApiOptions())
        {
        }

        /// <summary>
        /// Creates the service with options from dependency injection, its own <see cref="HttpClient"/> and no logging.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="options"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="options"/> is invalid, for example its language is not one of <see cref="MeteoSwissApiOptions.SupportedLanguages"/>.</exception>
        public MeteoSwissWeatherService(
            IOptions<MeteoSwissApiOptions> options)
          : this(NullLogger<MeteoSwissWeatherService>.Instance, options)
        {
        }

        /// <summary>
        /// Creates the service with the given options, its own <see cref="HttpClient"/> and no logging.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="options"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="options"/> is invalid, for example its language is not one of <see cref="MeteoSwissApiOptions.SupportedLanguages"/>.</exception>
        public MeteoSwissWeatherService(
            MeteoSwissApiOptions options)
          : this(NullLogger<MeteoSwissWeatherService>.Instance, options)
        {
        }

        /// <summary>
        /// Creates the service with options from dependency injection and its own <see cref="HttpClient"/>.
        /// This is the constructor that <c>AddMeteoSwissApi</c> uses.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="logger"/> or <paramref name="options"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="options"/> is invalid, for example its language is not one of <see cref="MeteoSwissApiOptions.SupportedLanguages"/>.</exception>
        public MeteoSwissWeatherService(
            ILogger<MeteoSwissWeatherService> logger,
            IOptions<MeteoSwissApiOptions> options)
            : this(logger, (options ?? throw new ArgumentNullException(nameof(options))).Value)
        {
        }

        /// <summary>
        /// Creates the service with the given options and its own <see cref="HttpClient"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="logger"/> or <paramref name="options"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="options"/> is invalid, for example its language is not one of <see cref="MeteoSwissApiOptions.SupportedLanguages"/>.</exception>
        public MeteoSwissWeatherService(
            ILogger<MeteoSwissWeatherService> logger,
            MeteoSwissApiOptions options)
            : this(logger, options, null)
        {
        }

        /// <summary>
        /// Creates the service with the given <see cref="HttpClient"/>, for example one with a shorter timeout or with your own message handlers.
        /// The service sets its headers per request, never changes the client and does not dispose it.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="logger"/>, <paramref name="httpClient"/> or <paramref name="options"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="options"/> is invalid, for example its language is not one of <see cref="MeteoSwissApiOptions.SupportedLanguages"/>.</exception>
        public MeteoSwissWeatherService(
            ILogger<MeteoSwissWeatherService> logger,
            HttpClient httpClient,
            MeteoSwissApiOptions options)
            : this(logger, options, httpClient ?? throw new ArgumentNullException(nameof(httpClient)))
        {
        }

        /// <summary>
        /// Creates the service with <paramref name="httpClient"/>, or with its own client, which it disposes, if <paramref name="httpClient"/> is null.
        /// </summary>
        private MeteoSwissWeatherService(
            ILogger<MeteoSwissWeatherService> logger,
            MeteoSwissApiOptions options,
            HttpClient? httpClient)
        {
            MeteoSwissApiOptionsValidator.ThrowIfInvalid(options, nameof(options));

            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.disposeHttpClient = httpClient is null;
            this.httpClient = httpClient ?? new HttpClient();
            this.verboseLogging = options.VerboseLogging;

            var acceptLanguage = new StringWithQualityHeaderValue(options.Language);
            this.configureHeaders = headers => headers.AcceptLanguage.Add(acceptLanguage);

            this.defaultWeatherIconMapping = new DefaultWeatherIconMapping(this.httpClient);
            this.defaultWarningIconMapping = new DefaultWarningIconMapping(this.httpClient);
        }

        /// <summary>
        /// Fails on JSON members the models do not declare, so that the integration tests notice when MeteoSwiss changes the API.
        /// </summary>
        internal bool DisallowUnmappedJsonMembers { get; set; }

        public async Task<WeatherInfo> GetCurrentWeatherAsync(int plz, CancellationToken cancellationToken = default)
        {
            var apiPlz = ToApiPlz(plz);

            var builder = new UriBuilder(ApiEndpoint)
            {
                Path = $"{ApiVersion}/plzDetail",
                Query = $"plz={apiPlz}"
            };

            var weatherInfo = await this.GetJsonAsync<WeatherInfo>(nameof(this.GetCurrentWeatherAsync), builder.Uri, cancellationToken).ConfigureAwait(false);

            var warnings = weatherInfo.Warnings
                .Select(w => (w.WarnType, w.WarnLevel))
                .Concat(weatherInfo.WarningsOverview.Select(w => (w.WarnType, w.WarnLevel)));
            this.LogUnknownWarnings(nameof(this.GetCurrentWeatherAsync), warnings);

            return weatherInfo;
        }

        public async Task<ForecastInfo> GetForecastAsync(int plz, CancellationToken cancellationToken = default)
        {
            var apiPlz = ToApiPlz(plz);

            var builder = new UriBuilder(ApiEndpoint)
            {
                Path = $"{ApiVersion}/forecast",
                Query = $"plz={apiPlz}"
            };

            var uri = builder.Uri;
            var forecastInfo = await this.GetJsonAsync<ForecastInfo>(nameof(this.GetForecastAsync), uri, cancellationToken).ConfigureAwait(false);

            // MeteoSwiss answers an unknown postal code with status 200 and a placeholder without values.
            // A response that drops the plz member would read as 0 too; such an API change deserves the same error.
            if (forecastInfo.Plz == UnknownPlz)
            {
                throw new MeteoSwissApiException($"MeteoSwiss does not know the PLZ {plz}.", uri, null, null);
            }

            var warnings = forecastInfo.WarningsOverview.Select(w => (w.WarnType, w.WarnLevel));
            this.LogUnknownWarnings(nameof(this.GetForecastAsync), warnings);

            return forecastInfo;
        }

        /// <summary>
        /// Reads the JSON response of <paramref name="uri"/> as <typeparamref name="T"/>.
        /// </summary>
        /// <exception cref="MeteoSwissApiException">The request failed or its response could not be read.</exception>
        private async Task<T> GetJsonAsync<T>(string methodName, Uri uri, CancellationToken cancellationToken) where T : class
        {
            this.logger.LogDebug("{MethodName}: GET {Uri}", methodName, uri);

            var options = JsonSerialization.GetOptions(this.DisallowUnmappedJsonMembers);

            if (this.verboseLogging)
            {
                // The content is logged, so it is read as a string before it is parsed.
                var responseJson = await this.httpClient.GetStringOrThrowAsync(uri, this.configureHeaders, cancellationToken).ConfigureAwait(false);
                this.logger.LogDebug("{MethodName} returned content: {Content}", methodName, responseJson);
                return JsonSerialization.Deserialize<T>(responseJson, uri, options);
            }

            return await this.httpClient.GetJsonOrThrowAsync<T>(uri, this.configureHeaders, options, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Converts <paramref name="plz"/> into the 6-digit form the API expects:
        /// a 4-digit PLZ gets the suffix 00 (8001 becomes 800100), a 6-digit PLZ already has its suffix (601002).
        /// </summary>
        private static string ToApiPlz(int plz)
        {
            if (plz is >= MinPlz and <= MaxPlz)
            {
                return $"{plz}{DefaultPlzSuffix}";
            }

            if (plz is >= MinPlzWithSuffix and <= MaxPlzWithSuffix)
            {
                return $"{plz}";
            }

            throw new ArgumentOutOfRangeException(
                nameof(plz),
                plz,
                "Expected a 4-digit PLZ such as 8001, or a 6-digit PLZ with suffix such as 800100.");
        }

        /// <summary>
        /// Logs warn types and levels this version does not know, so that new MeteoSwiss codes are noticed.
        /// </summary>
        private void LogUnknownWarnings(string methodName, IEnumerable<(WarnType WarnType, WarnLevel WarnLevel)> warnings)
        {
            foreach (var (warnType, warnLevel) in warnings.Distinct())
            {
                if (!warnType.IsKnown || !warnLevel.IsKnown)
                {
                    this.logger.LogDebug("{MethodName}: unknown warning with warnType={WarnType} and warnLevel={WarnLevel}", methodName, warnType.Value, warnLevel.Level);
                }
            }
        }

        public Task<Stream> GetWeatherIconAsync(int iconId, CancellationToken cancellationToken = default)
        {
            return this.GetWeatherIconAsync(iconId, this.defaultWeatherIconMapping, cancellationToken);
        }

        public Task<Stream> GetWeatherIconAsync(int iconId, IWeatherIconMapping weatherIconMapping, CancellationToken cancellationToken = default)
        {
            if (weatherIconMapping is null)
            {
                throw new ArgumentNullException(nameof(weatherIconMapping));
            }

            this.logger.LogDebug("GetWeatherIconAsync: iconId={IconId}, weatherIconMapping={WeatherIconMapping}", iconId, weatherIconMapping.GetType().Name);

            return weatherIconMapping.GetIconAsync(iconId, cancellationToken);
        }

        public Task<Stream> GetWarningIconAsync(WarnLevel warnLevel, CancellationToken cancellationToken = default)
        {
            return this.GetWarningIconAsync(warnLevel, this.defaultWarningIconMapping, cancellationToken);
        }

        public Task<Stream> GetWarningIconAsync(WarnLevel warnLevel, IWarningIconMapping warningIconMapping, CancellationToken cancellationToken = default)
        {
            if (warningIconMapping is null)
            {
                throw new ArgumentNullException(nameof(warningIconMapping));
            }

            this.logger.LogDebug("GetWarningIconAsync: warnLevel={WarnLevel}, warningIconMapping={WarningIconMapping}", warnLevel.Level, warningIconMapping.GetType().Name);

            return warningIconMapping.GetIconAsync(warnLevel, cancellationToken);
        }

        /// <summary>
        /// Disposes the <see cref="HttpClient"/> that the service created itself. A client passed to the constructor is not disposed:
        /// it belongs to the caller. Dispose the service after its last call has completed.
        /// </summary>
        public void Dispose()
        {
            if (this.disposeHttpClient)
            {
                this.httpClient.Dispose();
            }
        }
    }
}