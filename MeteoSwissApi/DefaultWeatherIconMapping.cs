using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace MeteoSwissApi
{
    public class DefaultWeatherIconMapping : IWeatherIconMapping
    {
        private const string ImageApiEndpoint = "https://www.meteoswiss.admin.ch/etc.clientlibs/internet/clientlibs/meteoswiss/resources/assets/images/icons/meteo/weather-symbols";

        private readonly HttpClient httpClient;

        public DefaultWeatherIconMapping(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<Stream> GetIconAsync(int iconId)
        {
            var iconUrl = $"{ImageApiEndpoint}/{iconId}.svg";

            var response = await this.httpClient.GetAsync(iconUrl);
            response.EnsureSuccessStatusCode();

            var responseStream = await response.Content.ReadAsStreamAsync();
            return responseStream;
        }
    }
}