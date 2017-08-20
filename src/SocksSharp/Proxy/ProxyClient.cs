using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SocksSharp.Proxy
{
    public class ProxyClient<T> : IProxyClient<T>, IDisposable where T : IProxy
    {
        private T client;        
        public ProxySettings Settings { get; set; }
                        
        public ProxyClient()
        {
            this.client = (T) Activator.CreateInstance(typeof(T));
        }
        
        /// <summary>
        /// Create connection via proxy to destination host
        /// </summary>
        /// <returns>Destination host NetworkStream</returns>
        /// <exception cref="System.InvalidOperationException">
        /// Value of <see cref="Host"/> equals <see langword="null"/> or empty.
        /// -or-
        /// Value of <see cref="Port"/> less than 1 or greater than 65535.
        /// -or-
        /// Value of <see cref="UserName"/> length greater than 255.
        /// -or-
        /// Value of <see cref="Password"/> length greater than 255.
        /// </exception>
        public NetworkStream GetDestinationStream(string destinationHost, int destinationPort)
        {
            TcpClient tcpClient = null;
            client.Settings = Settings;

            #region Create Connection

            tcpClient = new TcpClient();
            Exception connectException = null;
            var connectDoneEvent = new ManualResetEventSlim();

            try
            {
                tcpClient.BeginConnect(Settings.Host, Settings.Port, new AsyncCallback(
                    (ar) =>
                    {
                        if (tcpClient.Client != null)
                        {
                            try
                            {
                                tcpClient.EndConnect(ar);
                            }
                            catch (Exception ex)
                            {
                                connectException = ex;
                            }

                            connectDoneEvent.Set();
                        }
                    }), tcpClient
                );
            }
            catch (Exception ex)
            {
                tcpClient.Close();

                if (ex is SocketException || ex is SecurityException)
                {
                    throw new ProxyException("Failed to connect to proxy-server", ex);
                }

                throw;
            }

            if (!connectDoneEvent.Wait(Settings.ConnectTimeout))
            {
                tcpClient.Close();
                throw new ProxyException("Failed to connect to proxy-server");
            }

            if (connectException != null)
            {
                tcpClient.Close();

                if (connectException is SocketException)
                {
                    throw new ProxyException("Failed to connect to proxy-server", connectException);
                }
                else
                {
                    throw connectException;
                }
            }

            if (!tcpClient.Connected)
            {
                tcpClient.Close();
                throw new ProxyException("Failed to connect to proxy-server");
            }

            #endregion

            tcpClient.SendTimeout = Settings.ReadWriteTimeOut;
            tcpClient.ReceiveTimeout = Settings.ReadWriteTimeOut;
            
            var connectedTcpClient = client.CreateConnection(
                destinationHost,
                destinationPort,
                tcpClient);

            return connectedTcpClient.GetStream();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
                
    }

    public static class ProxyClient
    {
        public static bool TryParse<TOutput>(string proxyAddress, out IProxyClient<TOutput> proxyClient) where TOutput : IProxy
        {
            NetworkCredential credential = null;

            proxyClient = null;

            #region Parse Address

            if (String.IsNullOrEmpty(proxyAddress))
            {
                return false;
            }

            string[] values = proxyAddress.Split(':');

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

            var proxySettings = new ProxySettings();
            proxySettings.Host = host;
            proxySettings.Port = port;
            proxySettings.Credentials = credential;

            try
            {

                proxyClient = (IProxyClient<TOutput>)Activator.CreateInstance(typeof(ProxyClient<TOutput>));
                proxyClient.Settings = proxySettings;
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}