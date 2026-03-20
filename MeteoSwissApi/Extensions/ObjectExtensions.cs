using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using MeteoSwissApi.Serialization;

namespace MeteoSwissApi.Extensions
{
    internal static class ObjectExtensions
    {
        [return: NotNullIfNotNull(nameof(obj))]
        internal static T? Clone<T>(this object? obj) where T : class
        {
            if (obj == null)
            {
                return null;
            }

            var options = JsonSerialization.CreateOptions();
            var json = JsonSerializer.Serialize(obj, options);
            return JsonSerializer.Deserialize<T>(json, options)!;
        }
    }
}