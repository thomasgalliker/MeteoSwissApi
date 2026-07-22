namespace MeteoSwissApi
{
    public class DefaultWarningIconMapping : IWarningIconMapping
    {
        private const string ImageApiEndpoint = "https://www.meteoschweiz.admin.ch/static/resources/warn-symbols/{0}.svg";

        private readonly HttpClient httpClient;

        public DefaultWarningIconMapping(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<Stream> GetIconAsync(WarnLevel warnLevel)
        {
            if (warnLevel == WarnLevel.NoWarnLevel ||
                warnLevel == WarnLevel.Level1)
            {
                // MeteoSwiss does not provide warning icons below warn level 2
                return EmbeddedIcons.GetTransparentIcon();
            }

            var iconUrl = string.Format(ImageApiEndpoint, warnLevel.Level);

            var response = await this.httpClient.GetAsync(iconUrl);
            response.EnsureSuccessStatusCode();

            var responseStream = await response.Content.ReadAsStreamAsync();
            return responseStream;
        }
    }
}