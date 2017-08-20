using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

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
