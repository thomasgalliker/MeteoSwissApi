using System.Linq.Expressions;

namespace MeteoSwissApi.Tests.Utils
{
    /// <summary>
    /// Creates <see cref="HttpMessageHandler"/> mocks for the <see cref="HttpClient"/> of a service, so that tests run without network.
    /// </summary>
    internal static class HttpMessageHandlerMocks
    {
        /// <summary>
        /// Answers every request with <paramref name="statusCode"/> and <paramref name="content"/>.
        /// </summary>
        internal static Mock<HttpMessageHandler> Returning(HttpStatusCode statusCode, string content)
        {
            return Returning(_ => new HttpResponseMessage(statusCode) { Content = new StringContent(content) });
        }

        /// <summary>
        /// Answers every request with <paramref name="statusCode"/> and <paramref name="content"/>.
        /// </summary>
        internal static Mock<HttpMessageHandler> Returning(HttpStatusCode statusCode, byte[] content)
        {
            return Returning(_ => new HttpResponseMessage(statusCode) { Content = new ByteArrayContent(content) });
        }

        /// <summary>
        /// Answers every request with a new response from <paramref name="createResponse"/>.
        /// </summary>
        internal static Mock<HttpMessageHandler> Returning(Func<HttpRequestMessage, HttpResponseMessage> createResponse)
        {
            var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync((HttpRequestMessage request, CancellationToken _) => createResponse(request));

            return httpMessageHandlerMock;
        }

        /// <summary>
        /// Answers every request with <paramref name="sendAsync"/>, for tests that decide when a response arrives.
        /// </summary>
        internal static Mock<HttpMessageHandler> Returning(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> sendAsync)
        {
            var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Returns(sendAsync);

            return httpMessageHandlerMock;
        }

        /// <summary>
        /// Never answers: a request ends only when its cancellation token is cancelled.
        /// </summary>
        internal static Mock<HttpMessageHandler> WaitingForCancellation()
        {
            return Returning(async (HttpRequestMessage _, CancellationToken cancellationToken) =>
            {
                await Task.Delay(Timeout.Infinite, cancellationToken);
                return new HttpResponseMessage(HttpStatusCode.OK);
            });
        }

        /// <summary>
        /// Answers every request with status 200 and a body that never arrives, until the request is cancelled.
        /// </summary>
        internal static Mock<HttpMessageHandler> ReturningStallingContent()
        {
            return Returning(_ => new HttpResponseMessage(HttpStatusCode.OK) { Content = new StallingContent() });
        }

        internal static void VerifySendAsync(this Mock<HttpMessageHandler> httpMessageHandlerMock, Times times)
        {
            httpMessageHandlerMock.Protected()
                .Verify("SendAsync", times, ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
        }

        internal static void VerifySendAsync(this Mock<HttpMessageHandler> httpMessageHandlerMock, Times times, Expression<Func<HttpRequestMessage, bool>> match)
        {
            httpMessageHandlerMock.Protected()
                .Verify("SendAsync", times, ItExpr.Is(match), ItExpr.IsAny<CancellationToken>());
        }

        /// <summary>
        /// A body that never arrives: writing it waits until the cancellation token of the client is cancelled.
        /// </summary>
        private sealed class StallingContent : HttpContent
        {
            protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context)
            {
                return this.SerializeToStreamAsync(stream, context, CancellationToken.None);
            }

            protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context, CancellationToken cancellationToken)
            {
                return Task.Delay(Timeout.Infinite, cancellationToken);
            }

            protected override bool TryComputeLength(out long length)
            {
                length = 0;
                return false;
            }
        }
    }
}
