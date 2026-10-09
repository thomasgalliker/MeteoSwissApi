using System.Net;

namespace MeteoSwissApi
{
    public sealed class DefaultWarningIconMapping : IWarningIconMapping
    {
        private const string ImageApiEndpoint = "https://www.meteoschweiz.admin.ch/static/resources/warn-symbols/";

        private readonly HttpClient httpClient;

        /// <exception cref="ArgumentNullException"><paramref name="httpClient"/> is null.</exception>
        public DefaultWarningIconMapping(HttpClient httpClient)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<Stream> GetIconAsync(WarnLevel warnLevel, CancellationToken cancellationToken = default)
        {
            if (warnLevel == WarnLevel.NoWarnLevel ||
                warnLevel == WarnLevel.Level1)
            {
                // MeteoSwiss does not provide warning icons below warn level 2
                return EmbeddedIcons.GetTransparentIcon();
            }

            var iconUri = new Uri(FormattableString.Invariant($"{ImageApiEndpoint}{warnLevel.Level}.svg"), UriKind.Absolute);

            try
            {
                var iconBytes = await this.httpClient.GetBytesOrThrowAsync(iconUri, null, cancellationToken).ConfigureAwait(false);
                return new MemoryStream(iconBytes, writable: false);
            }
            catch (MeteoSwissApiException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                // MeteoSwiss has no icon for this warn level.
                return EmbeddedIcons.GetTransparentIcon();
            }
        }
    }
}