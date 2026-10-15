using System.Net.Http.Headers;
using System.Text.Json;

namespace MeteoSwissApi.Utils
{
    internal static class HttpClientExtensions
    {
        /// <summary>
        /// Returns the response body of <paramref name="requestUri"/> as string.
        /// </summary>
        internal static Task<string> GetStringOrThrowAsync(this HttpClient httpClient, Uri requestUri, Action<HttpRequestHeaders>? configureHeaders, CancellationToken cancellationToken)
        {
            return GetOrThrowAsync(httpClient, requestUri, configureHeaders, ReadAsStringAsync, cancellationToken);
        }

        /// <summary>
        /// Returns the response body of <paramref name="requestUri"/> as byte array.
        /// </summary>
        internal static Task<byte[]> GetBytesOrThrowAsync(this HttpClient httpClient, Uri requestUri, Action<HttpRequestHeaders>? configureHeaders, CancellationToken cancellationToken)
        {
            return GetOrThrowAsync(httpClient, requestUri, configureHeaders, ReadAsByteArrayAsync, cancellationToken);
        }

        /// <summary>
        /// Reads the JSON response body of <paramref name="requestUri"/> as <typeparamref name="T"/> from the buffered body, without a string copy.
        /// </summary>
        internal static Task<T> GetJsonOrThrowAsync<T>(this HttpClient httpClient, Uri requestUri, Action<HttpRequestHeaders>? configureHeaders, JsonSerializerOptions options, CancellationToken cancellationToken)
            where T : class
        {
            return GetOrThrowAsync(
                httpClient,
                requestUri,
                configureHeaders,
                async (content, ct) =>
                {
                    using (var stream = await ReadAsStreamAsync(content, ct).ConfigureAwait(false))
                    {
                        return await JsonSerialization.DeserializeAsync<T>(stream, requestUri, options, ct).ConfigureAwait(false);
                    }
                },
                cancellationToken);
        }

        private static async Task<T> GetOrThrowAsync<T>(
            HttpClient httpClient,
            Uri requestUri,
            Action<HttpRequestHeaders>? configureHeaders,
            Func<HttpContent, CancellationToken, Task<T>> readContentAsync,
            CancellationToken cancellationToken)
        {
            try
            {
                using (var request = new HttpRequestMessage(HttpMethod.Get, requestUri))
                {
                    configureHeaders?.Invoke(request.Headers);

                    // The client buffers the body before SendAsync returns, so HttpClient.Timeout covers the whole download.
                    // With ResponseHeadersRead it would cover the headers only, and a body that never arrives would hang a
                    // caller without a cancellation token. The responses are small, so the buffer costs nothing noticeable.
                    using (var response = await httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false))
                    {
                        if (!response.IsSuccessStatusCode)
                        {
                            throw new MeteoSwissApiException(
                                $"GET {requestUri} failed with status code {(int)response.StatusCode} ({response.StatusCode}).",
                                requestUri,
                                response.StatusCode,
                                null);
                        }

                        return await readContentAsync(response.Content, cancellationToken).ConfigureAwait(false);
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                throw new MeteoSwissApiException($"GET {requestUri} failed: {ex.Message}", requestUri, null, ex);
            }
        }

        private static Task<string> ReadAsStringAsync(HttpContent content, CancellationToken cancellationToken)
        {
#if NET5_0_OR_GREATER
            return content.ReadAsStringAsync(cancellationToken);
#else
            return content.ReadAsStringAsync();
#endif
        }

        private static Task<byte[]> ReadAsByteArrayAsync(HttpContent content, CancellationToken cancellationToken)
        {
#if NET5_0_OR_GREATER
            return content.ReadAsByteArrayAsync(cancellationToken);
#else
            return content.ReadAsByteArrayAsync();
#endif
        }

        private static Task<Stream> ReadAsStreamAsync(HttpContent content, CancellationToken cancellationToken)
        {
#if NET5_0_OR_GREATER
            return content.ReadAsStreamAsync(cancellationToken);
#else
            return content.ReadAsStreamAsync();
#endif
        }
    }
}
