using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace MeteoSwissApi
{
    public class MeteoSwissWeatherService : IMeteoSwissWeatherService
    {
        private static readonly Uri ApiEndpoint = new Uri("https://app-prod-ws.meteoswiss-app.ch", UriKind.Absolute);

        private const string ApiVersion = "v3";

        private const int PlzMinLength = 4;
        private const int PlzPaddingLength = 6;

        private readonly ILogger logger;
        private readonly HttpClient httpClient;
        private readonly IWeatherIconMapping defaultWeatherIconMapping;
        private readonly bool verboseLogging;
        private bool throwExceptionOnMissingJsonProperties;

        public MeteoSwissWeatherService()
            : this(new NullLogger<MeteoSwissWeatherService>())
        {
        }

        public MeteoSwissWeatherService(
            ILogger<MeteoSwissWeatherService> logger)
            : this(logger, new MeteoSwissApiOptions())
        {
        }

        public MeteoSwissWeatherService(
            IOptions<MeteoSwissApiOptions> options)
          : this(options.Value)
        {
        }

        public MeteoSwissWeatherService(
            MeteoSwissApiOptions options)
          : this(new NullLogger<MeteoSwissWeatherService>(), options)
        {
        }

        public MeteoSwissWeatherService(
            ILogger<MeteoSwissWeatherService> logger,
            IOptions<MeteoSwissApiOptions> options)
            : this(logger, options.Value)
        {
        }

        public MeteoSwissWeatherService(
            ILogger<MeteoSwissWeatherService> logger,
            MeteoSwissApiOptions options)
            : this(logger, new HttpClient(), options)
        {
        }

        public MeteoSwissWeatherService(
            ILogger<MeteoSwissWeatherService> logger,
            HttpClient httpClient,
            MeteoSwissApiOptions options)
        {
            this.logger = logger;
            this.verboseLogging = options.VerboseLogging;
            this.httpClient = httpClient;
            this.httpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue(options.Language));
            this.defaultWeatherIconMapping = new DefaultWeatherIconMapping(this.httpClient);
        }

        internal bool ThrowExceptionOnMissingJsonProperties
        {
            get => this.throwExceptionOnMissingJsonProperties;
            set => this.throwExceptionOnMissingJsonProperties = value;
        }

        public async Task<WeatherInfo> GetCurrentWeatherAsync(int plz)
        {
            var plzString = $"{plz}";

            if (plzString.Length < PlzMinLength)
            {
                throw new ArgumentException($"Parameter {nameof(plz)} must have a minimum length of {PlzMinLength}.", nameof(plz));
            }

            if (plzString.Length > PlzPaddingLength)
            {
                throw new ArgumentException($"Parameter {nameof(plz)} (padded) must not exceed a length of {PlzPaddingLength}.", nameof(plz));
            }

            this.logger.LogDebug($"GetCurrentWeatherAsync: plz={plz}");

            var plzPadded = PadPlz(plz);

            var builder = new UriBuilder(ApiEndpoint)
            {
                Path = $"{ApiVersion}/plzDetail",
                Query = $"plz={plzPadded}"
            };

            var uri = builder.ToString();
            this.logger.LogDebug($"GetCurrentWeatherAsync: GET {uri}");

            var response = await this.httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();

            if (this.verboseLogging)
            {
                this.logger.LogDebug($"GetCurrentWeatherAsync returned content:{Environment.NewLine}{responseJson}");
            }

            var weatherInfo = JsonSerializer.Deserialize<WeatherInfo>(responseJson, JsonSerialization.CreateOptions(this.throwExceptionOnMissingJsonProperties))!;
            return weatherInfo;
        }

        public async Task<ForecastInfo> GetForecastAsync(int plz)
        {
            var plzString = $"{plz}";

            if (plzString.Length < PlzMinLength)
            {
                throw new ArgumentException($"Parameter {nameof(plz)} must have a minimum length of {PlzMinLength}.", nameof(plz));
            }

            if (plzString.Length > PlzPaddingLength)
            {
                throw new ArgumentException($"Parameter {nameof(plz)} (padded) must not exceed a length of {PlzPaddingLength}.", nameof(plz));
            }

            this.logger.LogDebug($"GetForecastAsync: plz={plz}");

            var plzPadded = PadPlz(plz);

            var builder = new UriBuilder(ApiEndpoint)
            {
                Path = $"{ApiVersion}/forecast",
                Query = $"plz={plzPadded}"
            };

            var uri = builder.ToString();
            this.logger.LogDebug($"GetForecastAsync: GET {uri}");

            var response = await this.httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();

            if (this.verboseLogging)
            {
                this.logger.LogDebug($"GetForecastAsync returned content:{Environment.NewLine}{responseJson}");
            }

            var regionForecastResponse = JsonSerializer.Deserialize<ForecastInfo>(responseJson, JsonSerialization.CreateOptions(this.throwExceptionOnMissingJsonProperties))!;
            return regionForecastResponse;
        }

        private static string PadPlz(int plz)
        {
            return $"{plz}".PadRight(6, '0');
        }

        public async Task<Stream> GetWeatherIconAsync(int iconId, IWeatherIconMapping? weatherIconMapping = null)
        {
            if (weatherIconMapping == null)
            {
                weatherIconMapping = this.defaultWeatherIconMapping;
            }

            if (weatherIconMapping == null)
            {
                throw new ArgumentNullException(nameof(weatherIconMapping), $"Parameter {nameof(weatherIconMapping)} must not be null.");
            }

            this.logger.LogDebug($"GetWeatherIconAsync: iconId={iconId}, weatherIconMapping={weatherIconMapping.GetType().Name}");

            var imageStream = await weatherIconMapping.GetIconAsync(iconId);
            return imageStream;
        }
    }
}