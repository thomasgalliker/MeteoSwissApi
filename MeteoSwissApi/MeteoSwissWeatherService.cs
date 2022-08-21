using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using MeteoSwissApi.Models;
using MeteoSwissApi.Models.Converters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MeteoSwissApi
{
    public class MeteoSwissWeatherService : IMeteoSwissWeatherService
    {
        public const int PlzMinLength = 4;
        public const int PlzPaddingLength = 6;

        private readonly ILogger<MeteoSwissWeatherService> logger;
        private readonly HttpClient httpClient;
        private readonly IWeatherIconMapping defaultWeatherIconMapping;
        private readonly JsonSerializerSettings serializerSettings;
        private readonly Uri apiEndpoint;
        private readonly bool verboseLogging;

        public MeteoSwissWeatherService(ILogger<MeteoSwissWeatherService> logger, IMeteoSwissWeatherServiceConfiguration openWeatherMapConfiguration)
            : this(logger, new HttpClient(), openWeatherMapConfiguration)
        {
        }

        public MeteoSwissWeatherService(ILogger<MeteoSwissWeatherService> logger, HttpClient httpClient, IMeteoSwissWeatherServiceConfiguration openWeatherMapConfiguration)
        {
            this.logger = logger;
            this.apiEndpoint = new Uri(openWeatherMapConfiguration.ApiEndpoint, UriKind.Absolute);
            this.verboseLogging = openWeatherMapConfiguration.VerboseLogging;
            this.httpClient = httpClient;
            this.httpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue(openWeatherMapConfiguration.Language));
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

            var plzPadded = $"{plz}".PadRight(6, '0');

            var builder = new UriBuilder(this.apiEndpoint)
            {
                Path = "v1/plzDetail",
                Query = $"plz={plzPadded}"
            };

            var uri = builder.ToString();
            this.logger.LogDebug($"GetCurrentWeatherAsync: GET {uri}");

            var response = await this.httpClient.GetAsync(uri);
            _ = response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();

            if (this.verboseLogging)
            {
                this.logger.LogDebug($"GetCurrentWeatherAsync returned content:{Environment.NewLine}{responseJson}");
            }

            var weatherInfo = JsonConvert.DeserializeObject<WeatherInfo>(responseJson, this.serializerSettings);
            return weatherInfo;
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