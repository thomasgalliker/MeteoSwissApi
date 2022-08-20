using System.IO;
using System.Threading.Tasks;
using MeteoSwissApi.Models;

namespace MeteoSwissApi
{
    public interface IMeteoSwissWeatherService
    {
        Task<WeatherInfo> GetCurrentWeatherAsync(int plz);

        Task<Stream> GetWeatherIconAsync(int iconId, IWeatherIconMapping weatherIconMapping = null);
    }
}