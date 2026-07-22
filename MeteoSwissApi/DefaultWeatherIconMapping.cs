namespace MeteoSwissApi
{
    public class DefaultWeatherIconMapping : IWeatherIconMapping
    {
        private const string ImageApiEndpoint = "https://www.meteoschweiz.admin.ch/static/resources/weather-symbols/{0}.svg";

        /// <summary>
        /// MeteoSwiss encodes missing/unavailable values as 32767 (0x7FFF) in the JSON API.
        /// There is no downloadable icon for this id; an embedded transparent 1x1 icon is returned instead.
        /// </summary>
        private const int NoDataIconId = 32767;

        private readonly HttpClient httpClient;

        public DefaultWeatherIconMapping(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<Stream> GetIconAsync(int iconId)
        {
            if (iconId == NoDataIconId)
            {
                return EmbeddedIcons.GetTransparentIcon();
            }

            var iconUrl = string.Format(ImageApiEndpoint, iconId);

            var response = await this.httpClient.GetAsync(iconUrl);
            response.EnsureSuccessStatusCode();

            var responseStream = await response.Content.ReadAsStreamAsync();
            return responseStream;
        }
    }
}