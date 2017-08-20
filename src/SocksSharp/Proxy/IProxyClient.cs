using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SocksSharp.Proxy
{
    public interface IProxyClient<out T> where T : IProxy
    {
        ProxySettings Settings { get; set; }

        NetworkStream GetDestinationStream(string destinationHost, int destinationPort);
    }
}
