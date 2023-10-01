using Newtonsoft.Json;

namespace MeteoSwissApi.Extensions
{
    internal static class ObjectExtensions
    {
        internal static T Clone<T>(this object obj) where T : class
        {
            if (obj == null)
            {
                return null;
            }

            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(obj));
        }
    }
}
