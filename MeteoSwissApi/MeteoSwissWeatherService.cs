using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using MeteoSwissApi.Models;
using MeteoSwissApi.Models.Converters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace MeteoSwissApi
{
    public class MeteoSwissWeatherService : IMeteoSwissWeatherService
    {
        private static readonly Uri ApiEndpoint = new Uri("https://app-prod-ws.meteoswiss-app.ch", UriKind.Absolute);

        internal const int PlzMinLength = 4;
        internal const int PlzPaddingLength = 6;

        private readonly ILogger logger;
        private readonly HttpClient httpClient;
        private readonly IWeatherIconMapping defaultWeatherIconMapping;
        private readonly JsonSerializerSettings serializerSettings;
        private readonly bool verboseLogging;

        /// <summary>
        /// Initializes a new instance of the <see cref="MeteoSwissWeatherService"/> class.
        /// </summary>
        public MeteoSwissWeatherService()
            : this(new NullLogger<MeteoSwissWeatherService>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MeteoSwissWeatherService"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        public MeteoSwissWeatherService(
            ILogger<MeteoSwissWeatherService> logger)
            : this(logger, new MeteoSwissApiOptions())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MeteoSwissWeatherService"/> class.
        /// </summary>
        /// <param name="options">The service options.</param>
        public MeteoSwissWeatherService(
            IOptions<MeteoSwissApiOptions> options)
          : this(options.Value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MeteoSwissWeatherService"/> class.
        /// </summary>
        /// <param name="options">The service options.</param>
        public MeteoSwissWeatherService(
            MeteoSwissApiOptions options)
          : this(new NullLogger<MeteoSwissWeatherService>(), options)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MeteoSwissWeatherService"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="options">The service options.</param>
        public MeteoSwissWeatherService(
            ILogger<MeteoSwissWeatherService> logger,
            IOptions<MeteoSwissApiOptions> options)
            : this(logger, options.Value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MeteoSwissWeatherService"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="options">The service options.</param>
        public MeteoSwissWeatherService(
            ILogger<MeteoSwissWeatherService> logger,
            MeteoSwissApiOptions options)
            : this(logger, new HttpClient(), options)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MeteoSwissWeatherService"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="httpClient">The HttpClient instance.</param>
        /// <param name="options">The service options.</param>
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
            this.serializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
            };

            this.serializerSettings.Converters.Add(new TemperatureJsonConverter());
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
                Path = "v2/plzDetail",
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

            var weatherInfo = JsonConvert.DeserializeObject<WeatherInfo>(responseJson, this.serializerSettings);
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
                Path = "v2/forecast",
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

            var regionForecastResponse = JsonConvert.DeserializeObject<ForecastInfo>(responseJson, this.serializerSettings);
            return regionForecastResponse;
        }

        // TODO: https://app-prod-ws.meteoswiss-app.ch/v2/vorortdetail?plz=633000,630000&ws=

        // TODO: https://app-prod-ws.meteoswiss-app.ch/v1/stationOverview?station=CHZ

        private static string PadPlz(int plz)
        {
            return $"{plz}".PadRight(6, '0');
        }

        public async Task<Stream> GetWeatherIconAsync(int iconId, IWeatherIconMapping weatherIconMapping = null)
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