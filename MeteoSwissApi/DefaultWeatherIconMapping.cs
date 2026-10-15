using System.Net;

namespace MeteoSwissApi
{
    public sealed class DefaultWeatherIconMapping : IWeatherIconMapping
    {
        private const string ImageApiEndpoint = "https://www.meteoschweiz.admin.ch/static/resources/weather-symbols/";

        /// <summary>
        /// MeteoSwiss encodes missing/unavailable values as 32767 (0x7FFF) in the JSON API.
        /// There is no downloadable icon for this id; an embedded transparent 1x1 icon is returned instead.
        /// </summary>
        private const int NoDataIconId = 32767;

        private readonly HttpClient httpClient;

        /// <exception cref="ArgumentNullException"><paramref name="httpClient"/> is null.</exception>
        public DefaultWeatherIconMapping(HttpClient httpClient)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<Stream> GetIconAsync(int iconId, CancellationToken cancellationToken = default)
        {
            if (iconId == NoDataIconId)
            {
                return EmbeddedIcons.GetTransparentIcon();
            }

            var iconUri = new Uri(FormattableString.Invariant($"{ImageApiEndpoint}{iconId}.svg"), UriKind.Absolute);

            try
            {
                var iconBytes = await this.httpClient.GetBytesOrThrowAsync(iconUri, null, cancellationToken).ConfigureAwait(false);
                return new MemoryStream(iconBytes, writable: false);
            }
            catch (MeteoSwissApiException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                // MeteoSwiss has no icon for this icon id.
                return EmbeddedIcons.GetTransparentIcon();
            }
        }
    }
}