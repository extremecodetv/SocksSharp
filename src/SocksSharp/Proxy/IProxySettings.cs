using System.Net;

namespace SocksSharp.Proxy
{
    /// <summary>
    /// Provides an interface for proxy settings
    /// </summary>
    public interface IProxySettings
    {
        /// <summary>
        /// Gets or sets the credentials to submit to the proxy server for authentication.
        /// </summary>
        NetworkCredential Credentials { get; set; }

        /// <summary>
        /// Gets or sets a value of host or IP address for the proxy server
        /// </summary>
        string Host { get; set; }

        /// <summary>
        /// Gets or sets a value of Port for the proxy server
        /// </summary>
        int Port { get; set; }

        /// <summary>
        /// Gets or sets the amount of time a <see cref="ProxyClient{T}"/>
        /// will wait to connect to the proxy server
        /// </summary>
        int ConnectTimeout { get; set; }

        /// <summary>
        /// Gets or sets the amount of time a <see cref="ProxyClient{T}"/>
        /// will wait for read or wait data from the proxy server
        /// </summary>
        int ReadWriteTimeOut { get; set; }
    }
}
