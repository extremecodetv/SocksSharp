using System.Net.Sockets;

namespace SocksSharp.Proxy
{
    public interface IProxyClient<out T> where T : IProxy
    {
        ProxySettings Settings { get; set; }

        NetworkStream GetDestinationStream(string destinationHost, int destinationPort);
    }
}
