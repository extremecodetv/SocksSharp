using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SocksSharp.Proxy
{
    public class ProxySettings :  IProxySettings, IProxySettingsFluent
    {
        public NetworkCredential Credentials { get; set; }

        public string Host { get; set; }

        public int Port { get; set; }

        public int ConnectTimeout { get; set; } = 5000;

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
