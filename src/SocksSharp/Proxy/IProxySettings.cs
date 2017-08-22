using System.Net;

namespace SocksSharp.Proxy
{
    public interface IProxySettings
    {
        NetworkCredential Credentials { get; set; }

        string Host { get; set; }

        int Port { get; set; }

        int ConnectTimeout { get; set; }

        int ReadWriteTimeOut { get; set; }
    }
}
