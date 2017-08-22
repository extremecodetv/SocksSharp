using System.Net;

namespace SocksSharp.Proxy
{
    public interface IProxySettingsFluent
    {
        IProxySettingsFluent SetHost(string host);

        IProxySettingsFluent SetPort(int port);

        IProxySettingsFluent SetConnectionTimeout(int connectionTimeout);

        IProxySettingsFluent SetReadWriteTimeout(int readwriteTimeout);

        IProxySettingsFluent SetCredential(NetworkCredential credential);

        IProxySettingsFluent SetCredential(string username, string password);
    }
}
