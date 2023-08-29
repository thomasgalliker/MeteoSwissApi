using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using MeteoSwissApi.Utils;

namespace MeteoSwissApi
{
    public class HighContrastWeatherIconMapping : IWeatherIconMapping
    {
        private static readonly Assembly Assembly = typeof(HighContrastWeatherIconMapping).Assembly;
        private const string EmbeddedResourcePath = "outline.{0}.svg";

        public HighContrastWeatherIconMapping()
        {
        }

        public Task<Stream> GetIconAsync(int iconId)
        {
            var stream = ResourceLoader.Current.GetEmbeddedResourceStream(Assembly, string.Format(EmbeddedResourcePath, iconId));
            return Task.FromResult(stream);
        }
    }
}