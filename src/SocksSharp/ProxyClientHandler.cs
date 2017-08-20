using SocksSharp.Extensions;
using SocksSharp.Helpers;
using SocksSharp.Proxy;
using SocksSharp.Proxy.Request;
using SocksSharp.Proxy.Response;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SocksSharp
{
    public class ProxyClientHandler<T> : HttpMessageHandler, IDisposable where T : IProxy
    {
        private readonly IProxyClient<T> proxyClient;

        private Stream connectionCommonStream;
        private NetworkStream connectionNetworkStream;

        #region Properties

        public IProxyClient<T> Proxy => proxyClient;

        public bool UseProxy => true;

        public bool AllowAutoRedirect => false;

        public DecompressionMethods AutomaticDecompression
        {
            get => DecompressionMethods.GZip | DecompressionMethods.Deflate;
        }

        // public ClientCertificateOption ClientCertificateOption { get; set; }

        public bool UseCookies { get; set; }

        public CookieContainer CookieContainer { get; set; }

        public RemoteCertificateValidationCallback ServerCertificateCustomValidationCallback
        {
            get;
            set;
        }

        #endregion

        public ProxyClientHandler(ProxySettings proxySettings)
        {
            if(proxySettings == null)
            {
                throw new ArgumentNullException(nameof(proxySettings));
            }

            this.proxyClient = (IProxyClient<T>)Activator.CreateInstance(typeof(ProxyClient<T>));
            this.proxyClient.Settings = proxySettings;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return await Task.Run(async () =>
            {
                CreateConnection(request);

                await SendDataAsync(request, cancellationToken);
                var responseMessage = await ReceiveDataAsync(request, cancellationToken);

                return responseMessage;
            });
        }

        private async Task SendDataAsync(HttpRequestMessage request, CancellationToken ct)
        {
            byte[] buffer;
            var requestBuilder = new RequestBuilder(request);

            //Send starting line
            buffer = requestBuilder.BuildStartingLine();
            await connectionCommonStream.WriteAsync(buffer, 0, buffer.Length, ct);

            //Send headers
            buffer = requestBuilder.BuildHeaders();
            await connectionCommonStream.WriteAsync(buffer, 0, buffer.Length, ct);

            //Send Content
            await SendContentAsync(request, ct);
        }

        private async Task<HttpResponseMessage> ReceiveDataAsync(HttpRequestMessage request, CancellationToken ct)
        {
            var responseBuilder = new ResponseBuilder(1024);
            return await responseBuilder.GetResponseAsync(request, connectionNetworkStream, ct);
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

                    sslStream = ServerCertificateCustomValidationCallback == null
                        ? sslStream = new SslStream(connectionNetworkStream, false, null)
                        : sslStream = new SslStream(connectionNetworkStream, false, ServerCertificateCustomValidationCallback);

                    sslStream.AuthenticateAsClient(uri.Host);
                    connectionCommonStream = sslStream;
                }
                catch (Exception ex)
                {
                    if (ex is IOException || ex is AuthenticationException)
                    {
                        // throw NewHttpException(Resources.HttpException_FailedSslConnect, ex, HttpExceptionStatus.ConnectFailure);
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

            int offset = 0;
            int length = 1024;
            var buffer = await request.Content.ReadAsByteArrayAsync();

            while (offset < buffer.Length)
            {
                if (length > buffer.Length - offset)
                {
                    length = buffer.Length - offset;
                }

                await connectionCommonStream.WriteAsync(buffer, offset, length, ct);
                offset += length;
            }
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
    }
}
