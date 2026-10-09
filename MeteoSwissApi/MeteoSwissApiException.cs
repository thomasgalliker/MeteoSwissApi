using System.Net;

namespace MeteoSwissApi
{
    /// <summary>
    /// The exception that is thrown when a request to MeteoSwiss, SwissMetNet or SLF fails,
    /// or when its response cannot be read.
    /// </summary>
    public sealed class MeteoSwissApiException : Exception
    {
        /// <summary>
        /// Creates an exception with a default message.
        /// </summary>
        public MeteoSwissApiException()
        {
        }

        /// <summary>
        /// Creates an exception with the given <paramref name="message"/>.
        /// </summary>
        public MeteoSwissApiException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Creates an exception with the given <paramref name="message"/> that was caused by <paramref name="innerException"/>.
        /// </summary>
        public MeteoSwissApiException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Creates an exception for the request to <paramref name="requestUri"/>.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="requestUri">The URL of the request that failed.</param>
        /// <param name="statusCode">The HTTP status code of the response, or <c>null</c> if there is none.</param>
        /// <param name="innerException">The exception that caused this exception, or <c>null</c>.</param>
        public MeteoSwissApiException(string message, Uri? requestUri, HttpStatusCode? statusCode, Exception? innerException)
            : base(message, innerException)
        {
            this.RequestUri = requestUri;
            this.StatusCode = statusCode;
        }

        /// <summary>
        /// The URL of the request that failed, or <c>null</c> if no request was involved.
        /// </summary>
        public Uri? RequestUri { get; }

        /// <summary>
        /// The HTTP status code of the response, or <c>null</c> if no response was received
        /// or the response arrived but could not be read.
        /// </summary>
        public HttpStatusCode? StatusCode { get; }
    }
}
