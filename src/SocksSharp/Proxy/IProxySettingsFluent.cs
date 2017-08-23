using System.Net;

namespace SocksSharp.Proxy
{
    /// <summary>
    /// Provides an interface for fluent proxy settings
    /// </summary>
    public interface IProxySettingsFluent
    {
        /// <summary>
        /// Sets a value of host or IP address for the proxy server
        /// </summary>
        /// <param name="host">Host</param>
        /// <returns><see cref="IProxySettingsFluent"/></returns>
        IProxySettingsFluent SetHost(string host);

        /// <summary>
        /// Sets a value of Port for the proxy server
        /// </summary>
        /// <param name="port">Port</param>
        /// <returns><see cref="IProxySettingsFluent"/></returns>
        IProxySettingsFluent SetPort(int port);

        /// <summary>
        /// Gets or sets the amount of time a <see cref="ProxyClient{T}"/>
        /// will wait for read or wait data from the proxy server
        /// </summary>
        /// <param name="connectionTimeout">Connection timeout</param>
        /// <returns><see cref="IProxySettingsFluent"/></returns>
        IProxySettingsFluent SetConnectionTimeout(int connectionTimeout);

        /// <summary>
        /// Sets the amount of time a <see cref="ProxyClient{T}"/>
        /// will wait to connect to the proxy server
        /// </summary>
        /// <param name="readwriteTimeout">Read/Write timeout</param>
        /// <returns><see cref="IProxySettingsFluent"/></returns>
        IProxySettingsFluent SetReadWriteTimeout(int readwriteTimeout);

        /// <summary>
        /// Sets the credentials to submit to the proxy server for authentication
        /// </summary>
        /// <param name="credential">Credential</param>
        /// <returns><see cref="IProxySettingsFluent"/></returns>
        IProxySettingsFluent SetCredential(NetworkCredential credential);

        /// <summary>
        /// Sets the credentials to submit to the proxy server for authentication
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <returns><see cref="IProxySettingsFluent"/></returns>
        IProxySettingsFluent SetCredential(string username, string password);
    }
}
