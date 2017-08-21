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

        public IProxySettingsFluent SetConnectionTimeout(int connectionTimeout)
        {
            throw new NotImplementedException();
        }

        public IProxySettingsFluent SetCredential(NetworkCredential credential)
        {
            throw new NotImplementedException();
        }

        public IProxySettingsFluent SetCredential(string username, string password)
        {
            throw new NotImplementedException();
        }

        public IProxySettingsFluent SetHost(string host)
        {
            throw new NotImplementedException();
        }

        public IProxySettingsFluent SetPort(int port)
        {
            throw new NotImplementedException();
        }

        public IProxySettingsFluent SetReadWriteTimeout(int readwriteTimeout)
        {
            throw new NotImplementedException();
        }

#endregion
    }
}
