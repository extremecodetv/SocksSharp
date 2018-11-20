using System;
using System.Dynamic;
using System.IO;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;
using SocksSharp.Proxy;
using SocksSharp.Proxy.Request;
using SocksSharp.Proxy.Response;

namespace SocksSharp
{
    /// <summary>
    /// Represents <see cref="HttpMessageHandler"/> with <see cref="IProxyClient{T}"/>
    /// to provide the <see cref="HttpClient"/> support for <see cref="IProxy"/> proxy type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ProxyClientHandler<T> : HttpMessageHandler, IDisposable where T : IProxy
    {
        private readonly IProxyClient<T> proxyClient;

        private Stream connectionCommonStream;
        private NetworkStream connectionNetworkStream;

        #region Properties

        /// <summary>
        /// Gets a current ProxyClient 
        /// </summary>
        public IProxyClient<T> Proxy => proxyClient;

        /// <summary>
        /// Gets a value that indicates whether the handler uses a proxy for requests.
        /// </summary>
        public bool UseProxy => true;

        /// <summary>
        /// Gets a value that indicates whether the handler supports proxy settings.
        /// </summary>
        public bool SupportsProxy => true;

        /// <summary>
        /// Gets a value that indicates whether the handler should follow redirection responses.
        /// </summary>
        public bool AllowAutoRedirect { get; set; }

        /// <summary>
        /// Gets a value that indicates whether the handler supports 
        /// configuration settings for the <see cref="AllowAutoRedirect"/>
        /// </summary>
        public bool SupportsRedirectConfiguration => true;

        /// <summary>
        /// Gets the type of decompression method used by the handler for automatic 
        /// decompression of the HTTP content response.
        /// </summary>
        /// <remarks>
        /// Support GZip and Deflate encoding automatically
        /// </remarks>
        public DecompressionMethods AutomaticDecompression
        {
            get => DecompressionMethods.GZip | DecompressionMethods.Deflate;
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the handler uses the CookieContainer
        /// property to store server cookies and uses these cookies when sending requests.
        /// </summary>
        public bool UseCookies { get; set; }

        /// <summary>
        /// Gets or sets the cookie container used to store server cookies by the handler.
        /// </summary>
        public CookieContainer CookieContainer { get; set; }

        /// <summary>
        /// Gets or sets delegate to verifies the remote Secure Sockets Layer (SSL) 
        /// certificate used for authentication.
        /// </summary>
        public RemoteCertificateValidationCallback ServerCertificateCustomValidationCallback { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyClientHandler{T}"/> with <see cref="ProxySettings"/> settings
        /// </summary>
        /// <param name="proxySettings">Proxy settings</param>
        /// <exception cref="ArgumentNullException">
        /// Value of parameter is <see langword="null"/>
        /// </exception>
        public ProxyClientHandler(ProxySettings proxySettings)
        {
            if (proxySettings == null)
            {
                throw new ArgumentNullException(nameof(proxySettings));
            }

            this.proxyClient = (IProxyClient<T>) Activator.CreateInstance(typeof(ProxyClient<T>));

            this.proxyClient.Settings = proxySettings;
        }

        /// <summary>
        /// Creates an instance of HttpResponseMessage based on the information provided in the HttpRequestMessage as an operation that will not block.
        /// </summary>
        /// <param name="request">The HTTP request message.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <returns>Instance of <see cref="HttpResponseMessage"/> containing http response</returns>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return await Task.Run(async () =>
            {
                if (UseCookies && CookieContainer == null)
                {
                    throw new ArgumentNullException(nameof(CookieContainer));
                }

                CreateConnection(request);
                await SendDataAsync(request, cancellationToken).ConfigureAwait(false);
                var responseMessage = await ReceiveDataAsync(request, cancellationToken).ConfigureAwait(false);

                if ((responseMessage.StatusCode == HttpStatusCode.Moved ||
                     responseMessage.StatusCode == HttpStatusCode.MovedPermanently ||
                     responseMessage.StatusCode == HttpStatusCode.Redirect) && AllowAutoRedirect)
                {
                    request.RequestUri = responseMessage.Headers.Location;

                    return await SendAsync(request, cancellationToken).ConfigureAwait(false);
                }

                return responseMessage;
            }).ConfigureAwait(false);
        }

        #region Methods (private)

        private async Task SendDataAsync(HttpRequestMessage request, CancellationToken ct)
        {
            byte[] buffer;
            var hasContent = request.Content != null;
            var requestBuilder = UseCookies
                ? new RequestBuilder(request, CookieContainer)
                : new RequestBuilder(request);

//Send starting line
            buffer = requestBuilder.BuildStartingLine();
            await connectionCommonStream.WriteAsync(buffer, 0, buffer.Length, ct).ConfigureAwait(false);

//Send headers
            buffer = requestBuilder.BuildHeaders(hasContent);
            await connectionCommonStream.WriteAsync(buffer, 0, buffer.Length, ct).ConfigureAwait(false);
            if (hasContent)
            {
                await SendContentAsync(request, ct).ConfigureAwait(false);
            }
        }

        private async Task<HttpResponseMessage> ReceiveDataAsync(HttpRequestMessage request, CancellationToken ct)
        {
            var responseBuilder = UseCookies
                ? new ResponseBuilder(1024, CookieContainer, request.RequestUri)
                : new ResponseBuilder(1024);
            return await responseBuilder.GetResponseAsync(request, connectionCommonStream, ct);
        }

        private void CreateConnection(HttpRequestMessage request)
        {
            Uri uri = request.RequestUri;
            connectionNetworkStream = proxyClient.GetDestinationStream(uri.Host, uri.Port);
            if (uri.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    SslStream sslStream;
                    sslStream = new SslStream(connectionNetworkStream, false, ServerCertificateCustomValidationCallback);
                    sslStream.AuthenticateAsClient(uri.Host);
                    connectionCommonStream = sslStream;
                }
                catch (Exception ex)
                {
                    if (ex is IOException || ex is AuthenticationException)
                    {
                        throw new ProxyException("Failed SSL connect");
                    }

                    throw;
                }
            }
            else
            {
                connectionCommonStream = connectionNetworkStream;
            }
        }

        private async Task SendContentAsync(HttpRequestMessage request, CancellationToken ct)
        {
            var buffer = await request.Content.ReadAsByteArrayAsync();
            await connectionCommonStream.WriteAsync(buffer, 0, buffer.Length, ct).ConfigureAwait(false);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                connectionCommonStream?.Dispose();
                connectionNetworkStream?.Dispose();
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}