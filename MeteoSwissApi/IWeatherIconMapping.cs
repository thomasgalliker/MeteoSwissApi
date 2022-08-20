using System.IO;
using System.Threading.Tasks;

namespace MeteoSwissApi
{
    public interface IWeatherIconMapping
    {
        Task<Stream> GetIconAsync(int iconId);
    }
}