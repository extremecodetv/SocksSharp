using System.Net.Sockets;

namespace SocksSharp.Proxy
{
    /// <summary>
    /// Provides an interface for <see cref="ProxyClient{T}"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IProxyClient<out T> where T : IProxy
    {
        /// <summary>
        /// Gets or sets proxy settings for client
        /// </summary>
        ProxySettings Settings { get; set; }

        /// <summary>
        /// Create connection via proxy to destination host
        /// </summary>
        /// <returns>Destination <see cref="NetworkStream"/></returns>
        NetworkStream GetDestinationStream(string destinationHost, int destinationPort);
    }
}
