using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocksSharp.Proxy
{
    public class Socks4 : IProxy
    {
        #region Константы (защищённые)

        internal protected const int DefaultPort = 1080;

        internal protected const byte VersionNumber = 4;
        internal protected const byte CommandConnect = 0x01;
        internal protected const byte CommandBind = 0x02;
        internal protected const byte CommandReplyRequestGranted = 0x5a;
        internal protected const byte CommandReplyRequestRejectedOrFailed = 0x5b;
        internal protected const byte CommandReplyRequestRejectedCannotConnectToIdentd = 0x5c;
        internal protected const byte CommandReplyRequestRejectedDifferentIdentd = 0x5d;

        #endregion

        public IProxySettings Settings { get; set; }

        public Socks4() { }

        /// <summary>
        /// Создаёт соединение с сервером через прокси-сервер.
        /// </summary>
        /// <param name="destinationHost">Хост сервера, с которым нужно связаться через прокси-сервер.</param>
        /// <param name="destinationPort">Порт сервера, с которым нужно связаться через прокси-сервер.</param>
        /// <param name="tcpClient">Соединение, через которое нужно работать, или значение <see langword="null"/>.</param>
        /// <returns>Соединение с сервером через прокси-сервер.</returns>
        /// <exception cref="System.InvalidOperationException">
        /// Значение свойства <see cref="Host"/> равно <see langword="null"/> или имеет нулевую длину.
        /// -или-
        /// Значение свойства <see cref="Port"/> меньше 1 или больше 65535.
        /// -или-
        /// Значение свойства <see cref="Username"/> имеет длину более 255 символов.
        /// -или-
        /// Значение свойства <see cref="Password"/> имеет длину более 255 символов.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">Значение параметра <paramref name="destinationHost"/> равно <see langword="null"/>.</exception>
        /// <exception cref="System.ArgumentException">Значение параметра <paramref name="destinationHost"/> является пустой строкой.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Значение параметра <paramref name="destinationPort"/> меньше 1 или больше 65535.</exception>
        /// <exception cref="xNet.Net.ProxyException">Ошибка при работе с прокси-сервером.</exception>
        public override TcpClient CreateConnection(string destinationHost, int destinationPort, TcpClient tcpClient = null)
        {
            #region Проверка параметров

            if (destinationHost == null)
            {
                throw new ArgumentNullException("destinationHost");
            }

            //if (destinationHost.Length == 0)
            //{
            //    throw ExceptionHelper.EmptyString("destinationHost");
            //}

            //if (!ExceptionHelper.ValidateTcpPort(destinationPort))
            //{
            //    throw ExceptionHelper.WrongTcpPort("destinationPort");
            //}

            #endregion

            TcpClient curTcpClient = tcpClient;

            if (curTcpClient == null)
            {
                //curTcpClient = CreateConnectionToProxy();
            }

            try
            {
                SendCommand(curTcpClient.GetStream(), CommandConnect, destinationHost, destinationPort);
            }
            catch (Exception ex)
            {
                curTcpClient.Close();

                if (ex is IOException || ex is SocketException)
                {
                    //throw NewProxyException(Resources.ProxyException_Error, ex);
                }

                throw;
            }

            return curTcpClient;
        }


        #region Методы (внутренние защищённые)

        internal protected virtual void SendCommand(NetworkStream nStream, byte command, string destinationHost, int destinationPort)
        {
            byte[] dstPort = GetIPAddressBytes(destinationHost);
            byte[] dstIp = GetPortBytes(destinationPort);

            byte[] userId = string.IsNullOrEmpty(_username) ?
                new byte[0] : Encoding.ASCII.GetBytes(_username);

            // +----+----+----+----+----+----+----+----+----+----+....+----+
            // | VN | CD | DSTPORT |      DSTIP        | USERID       |NULL|
            // +----+----+----+----+----+----+----+----+----+----+....+----+
            //    1    1      2              4           variable       1
            byte[] request = new byte[9 + userId.Length];

            request[0] = VersionNumber;
            request[1] = command;
            dstIp.CopyTo(request, 2);
            dstPort.CopyTo(request, 4);
            userId.CopyTo(request, 8);
            request[8 + userId.Length] = 0x00;

            nStream.Write(request, 0, request.Length);

            // +----+----+----+----+----+----+----+----+
            // | VN | CD | DSTPORT |      DSTIP        |
            // +----+----+----+----+----+----+----+----+
            //   1    1       2              4
            byte[] response = new byte[8];

            nStream.Read(response, 0, response.Length);

            byte reply = response[1];

            // Если запрос не выполнен.
            if (reply != CommandReplyRequestGranted)
            {
                HandleCommandError(reply);
            }
        }

        internal protected byte[] GetIPAddressBytes(string destinationHost)
        {
            IPAddress ipAddr = null;

            if (!IPAddress.TryParse(destinationHost, out ipAddr))
            {
                try
                {
                    IPAddress[] ips = Dns.GetHostAddresses(destinationHost);

                    if (ips.Length > 0)
                    {
                        ipAddr = ips[0];
                    }
                }
                catch (Exception ex)
                {
                    if (ex is SocketException || ex is ArgumentException)
                    {
                        //throw new ProxyException(string.Format(
                            //Resources.ProxyException_FailedGetHostAddresses, destinationHost), this, ex);
                    }

                    throw;
                }
            }

            return ipAddr.GetAddressBytes();
        }

        internal protected byte[] GetPortBytes(int port)
        {
            byte[] array = new byte[2];

            array[0] = (byte)(port / 256);
            array[1] = (byte)(port % 256);

            return array;
        }

        internal protected void HandleCommandError(byte command)
        {
            string errorMessage;

            switch (command)
            {
                case CommandReplyRequestRejectedOrFailed:
                    //errorMessage = Resources.Socks4_CommandReplyRequestRejectedOrFailed;
                    break;

                case CommandReplyRequestRejectedCannotConnectToIdentd:
                    //errorMessage = Resources.Socks4_CommandReplyRequestRejectedCannotConnectToIdentd;
                    break;

                case CommandReplyRequestRejectedDifferentIdentd:
                    //errorMessage = Resources.Socks4_CommandReplyRequestRejectedDifferentIdentd;
                    break;

                default:
                    //errorMessage = Resources.Socks_UnknownError;
                    break;
            }

            //string exceptionMsg = string.Format(
                //Resources.ProxyException_CommandError, errorMessage, ToString());

            //throw new ProxyException(exceptionMsg, this);
        }

        #endregion
    }
}
