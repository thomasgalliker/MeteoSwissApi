using System.Text.Json;
using System.Text.Json.Serialization;

namespace MeteoSwissApi.Serialization
{
    internal static class JsonSerialization
    {
        internal static JsonSerializerOptions CreateOptions(bool disallowUnmappedMembers = false)
        {
            var options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                PropertyNameCaseInsensitive = false,
                UnmappedMemberHandling = disallowUnmappedMembers
                    ? JsonUnmappedMemberHandling.Disallow
                    : JsonUnmappedMemberHandling.Skip,
            };

            options.Converters.Add(new TemperatureJsonConverter());
            return options;
        }
    }
}
