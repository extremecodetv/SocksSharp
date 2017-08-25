using System;
using System.Net;

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

        /// <summary>
        /// Converts the string representation of a <see cref="ProxySettings"/>. 
        /// A return value indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="proxy">A string containing proxy settings</param>
        /// <param name="proxySettings">When this method returns,
        /// contains the instance of the <see cref="ProxySettings"/> value equivalent of the number contained in proxy, 
        /// if the conversion succeeded, or <see cref="null"/> if the conversion failed.</param>
        /// <returns><see cref="true"/> if s was converted successfully; otherwise, <see cref="false"/>.</returns>
        /// <remarks>String must be in one of this format 
        /// host:port
        /// - or -
        /// host:port:username
        /// - or -
        /// host:port:username:password
        /// </remarks>
        public static bool TryParse(string proxy, out ProxySettings proxySettings)
        {
            NetworkCredential credential = null;

            proxySettings = null;

            #region Parse Address

            if (String.IsNullOrEmpty(proxy))
            {
                return false;
            }

            string[] values = proxy.Split(':');

            int port = 1080;
            string host = values[0];

            if (values.Length >= 2)
            {
                if (!int.TryParse(values[1], out port))
                {
                    return false;
                }
            }
            #endregion

            #region Parse Credential

            string username = String.Empty;
            string password = String.Empty;

            if (values.Length >= 3)
            {
                credential = new NetworkCredential();

                username = values[2];

                if (values.Length >= 4)
                {
                    password = values[3];
                }

                if (!String.IsNullOrEmpty(username))
                {
                    credential.UserName = username;
                }

                if (!String.IsNullOrEmpty(password))
                {
                    credential.Password = password;
                }
            }

            #endregion

            proxySettings = new ProxySettings();
            proxySettings.Host = host;
            proxySettings.Port = port;
            proxySettings.Credentials = credential;

            return true;
        }
    }
}
