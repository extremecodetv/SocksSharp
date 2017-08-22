using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SocksSharp.Proxy.Response
{
    public interface IResponseBuilder
    {
        int ReceiveTimeout { get; set; }

        Task<HttpResponseMessage> GetResponseAsync(HttpRequestMessage request, NetworkStream stream);

        Task<HttpResponseMessage> GetResponseAsync(HttpRequestMessage request, NetworkStream stream, CancellationToken cancellationToken);
    }
}
