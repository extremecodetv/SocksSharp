using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SocksSharp.Proxy
{
    /// <summary>
    /// Represents the settings for <see cref="ProxyClient{T}"/>
    /// </summary>
    public class ProxySettings :  IProxySettings, IProxySettingsFluent
    {
        /// <summary>
        /// Gets or sets the credentials to submit to the proxy server for authentication.
        /// </summary>
        public NetworkCredential Credentials { get; set; }

        /// <summary>
        /// Gets or sets a value of host or IP address for the proxy server
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Gets or sets a value of Port for the proxy server
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Gets or sets the amount of time a <see cref="ProxyClient{T}"/>
        /// will wait to connect to the proxy server
        /// </summary>
        public int ConnectTimeout { get; set; } = 5000;

        /// <summary>
        /// Gets or sets the amount of time a <see cref="ProxyClient{T}"/>
        /// will wait for read or wait data from the proxy server
        /// </summary>
        public int ReadWriteTimeOut { get; set; } = 10000;

        #region Fluent

        /// <summary>
        /// Sets the credentials to submit to the proxy server for authentication
        /// </summary>
        /// <param name="credential">Credential</param>
        /// <returns><see cref="IProxySettingsFluent"/></returns>
        public IProxySettingsFluent SetCredential(NetworkCredential credential)
        {
            Credentials = credential;
            return this;
        }

        /// <summary>
        /// Sets the credentials to submit to the proxy server for authentication
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <returns><see cref="IProxySettingsFluent"/></returns>
        public IProxySettingsFluent SetCredential(string username, string password)
        {
            Credentials = new NetworkCredential(username, password);
            return this;
        }

        /// <summary>
        /// Sets a value of host or IP address for the proxy server
        /// </summary>
        /// <param name="host">Host</param>
        /// <returns><see cref="IProxySettingsFluent"/></returns>
        public IProxySettingsFluent SetHost(string host)
        {
            Host = host;
            return this;
        }

        /// <summary>
        /// Sets a value of Port for the proxy server
        /// </summary>
        /// <param name="port">Port</param>
        /// <returns><see cref="IProxySettingsFluent"/></returns>
        public IProxySettingsFluent SetPort(int port)
        {
            Port = port;
            return this;
        }

        /// <summary>
        /// Sets the amount of time a <see cref="ProxyClient{T}"/>
        /// will wait to connect to the proxy server
        /// </summary>
        /// <param name="readwriteTimeout">Read/Write timeout</param>
        /// <returns><see cref="IProxySettingsFluent"/></returns>
        public IProxySettingsFluent SetReadWriteTimeout(int readwriteTimeout)
        {
            ReadWriteTimeOut = readwriteTimeout;
            return this;
        }

        /// <summary>
        /// Gets or sets the amount of time a <see cref="ProxyClient{T}"/>
        /// will wait for read or wait data from the proxy server
        /// </summary>
        /// <param name="connectionTimeout">Connection timeout</param>
        /// <returns><see cref="IProxySettingsFluent"/></returns>
        public IProxySettingsFluent SetConnectionTimeout(int connectionTimeout)
        {
            ConnectTimeout = connectionTimeout;
            return this;
        }

        #endregion
    }
}
