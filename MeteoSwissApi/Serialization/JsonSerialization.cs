using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;

namespace MeteoSwissApi.Serialization
{
    internal static class JsonSerialization
    {
        /// <summary>
        /// Reads API responses and skips JSON members the models do not declare.
        /// </summary>
        internal static readonly JsonSerializerOptions Default = CreateOptions(JsonUnmappedMemberHandling.Skip);

        /// <summary>
        /// Reads API responses and fails on JSON members the models do not declare, so that API changes are noticed early.
        /// </summary>
        internal static readonly JsonSerializerOptions Strict = CreateOptions(JsonUnmappedMemberHandling.Disallow);

        /// <summary>
        /// Returns <see cref="Strict"/> if <paramref name="disallowUnmappedMembers"/> is <c>true</c>, otherwise <see cref="Default"/>.
        /// </summary>
        internal static JsonSerializerOptions GetOptions(bool disallowUnmappedMembers)
        {
            return disallowUnmappedMembers ? Strict : Default;
        }

        private static JsonSerializerOptions CreateOptions(JsonUnmappedMemberHandling unmappedMemberHandling)
        {
            var options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                PropertyNameCaseInsensitive = false,
                UnmappedMemberHandling = unmappedMemberHandling,
            };

            options.Converters.Add(new TemperatureJsonConverter());
            options.Converters.Add(new SlfStationTypeJsonConverter());
            return options;
        }

        /// <summary>
        /// Reads the JSON response of <paramref name="requestUri"/>.
        /// </summary>
        /// <exception cref="MeteoSwissApiException">The response is empty or cannot be read as <typeparamref name="T"/>.</exception>
        internal static T Deserialize<T>(string json, Uri requestUri, JsonSerializerOptions options) where T : class
        {
            T? value;

            try
            {
                value = JsonSerializer.Deserialize<T>(json, options);
            }
            catch (JsonException ex)
            {
                throw CreateUnreadableResponseException<T>(requestUri, ex);
            }

            return value ?? throw CreateEmptyResponseException(requestUri);
        }

        /// <summary>
        /// Reads the JSON response of <paramref name="requestUri"/> from <paramref name="stream"/>.
        /// </summary>
        /// <exception cref="MeteoSwissApiException">The response is empty or cannot be read as <typeparamref name="T"/>.</exception>
        internal static async Task<T> DeserializeAsync<T>(Stream stream, Uri requestUri, JsonSerializerOptions options, CancellationToken cancellationToken)
            where T : class
        {
            T? value;

            try
            {
                value = await JsonSerializer.DeserializeAsync<T>(stream, options, cancellationToken).ConfigureAwait(false);
            }
            catch (JsonException ex)
            {
                throw CreateUnreadableResponseException<T>(requestUri, ex);
            }

            return value ?? throw CreateEmptyResponseException(requestUri);
        }

        private static MeteoSwissApiException CreateUnreadableResponseException<T>(Uri requestUri, JsonException exception)
        {
            return new MeteoSwissApiException($"The response of GET {requestUri} could not be read as {typeof(T).Name}.", requestUri, null, exception);
        }

        private static MeteoSwissApiException CreateEmptyResponseException(Uri requestUri)
        {
            return new MeteoSwissApiException($"GET {requestUri} returned an empty response.", requestUri, null, null);
        }
    }
}
