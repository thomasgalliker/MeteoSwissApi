using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace MeteoSwissApi
{
    public class DefaultWeatherIconMapping : IWeatherIconMapping
    {
        private const string ImageApiEndpoint = "https://www.meteoswiss.admin.ch/static/product/resources/weather-symbols/{0}.svg";

        private readonly HttpClient httpClient;

        public DefaultWeatherIconMapping(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<Stream> GetIconAsync(int iconId)
        {
            var iconUrl = string.Format(ImageApiEndpoint, iconId);

            var response = await this.httpClient.GetAsync(iconUrl);
            response.EnsureSuccessStatusCode();

            var responseStream = await response.Content.ReadAsStreamAsync();
            return responseStream;
        }
    }
}