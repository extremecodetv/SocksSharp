using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocksSharp.Proxy
{
    public interface IProxy
    {
        IProxySettings Settings { get; set; }

        TcpClient CreateConnection(string destinationHost, int destinationPort, TcpClient client);
    }
}
