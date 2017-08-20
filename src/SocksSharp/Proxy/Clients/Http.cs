using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SocksSharp.Proxy
{
    public class Http : IProxy
    {
        #region Константы (закрытые)

        private const int BufferSize = 50;
        private const int DefaultPort = 8080;

        #endregion
        
        public IProxySettings Settings { get; set; }
        
        #region Методы (открытые)

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
        /// <remarks>Если порт сервера неравен 80, то для подключения используется метод 'CONNECT'.</remarks>
        public TcpClient CreateConnection(string destinationHost, int destinationPort, TcpClient tcpClient)
        {
            //CheckState();

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
            
            if (destinationPort != 80)
            {
                HttpStatusCode statusCode = HttpStatusCode.OK;

                try
                {
                    NetworkStream nStream = curTcpClient.GetStream();

                    SendConnectionCommand(nStream, destinationHost, destinationPort);
                    statusCode = HttpStatusCode.OK; ReceiveResponse(nStream);
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

                if (statusCode != HttpStatusCode.OK)
                {
                    curTcpClient.Close();

                    //throw new ProxyException(string.Format(
                        //Resources.ProxyException_ReceivedWrongStatusCode, statusCode, ToString()), this);
                }
            }

            return curTcpClient;
        }

        #endregion
        
        #region Методы (закрытые)

        private string GenerateAuthorizationHeader()
        {
            //if (!string.IsNullOrEmpty(_username) || !string.IsNullOrEmpty(_password))
            //{
            //    string data = Convert.ToBase64String(Encoding.UTF8.GetBytes(
            //        string.Format("{0}:{1}", _username, _password)));

            //    return string.Format("Proxy-Authorization: Basic {0}\r\n", data);
            //}

            return string.Empty;
        }

        private void SendConnectionCommand(NetworkStream nStream, string destinationHost, int destinationPort)
        {
            var commandBuilder = new StringBuilder();

            commandBuilder.AppendFormat("CONNECT {0}:{1} HTTP/1.1\r\n", destinationHost, destinationPort);
            commandBuilder.AppendFormat(GenerateAuthorizationHeader());
            commandBuilder.AppendLine();

            byte[] buffer = Encoding.ASCII.GetBytes(commandBuilder.ToString());

            nStream.Write(buffer, 0, buffer.Length);
        }

        private HttpStatusCode ReceiveResponse(NetworkStream nStream)
        {
            byte[] buffer = new byte[BufferSize];
            var responseBuilder = new StringBuilder();

            WaitData(nStream);

            do
            {
                int bytesRead = nStream.Read(buffer, 0, BufferSize);
                responseBuilder.Append(Encoding.ASCII.GetString(buffer, 0, bytesRead));
            } while (nStream.DataAvailable);

            string response = responseBuilder.ToString();

            if (response.Length == 0)
            {
                //throw NewProxyException(Resources.ProxyException_ReceivedEmptyResponse);
            }

            // Выделяем строку статуса. Пример: HTTP/1.1 200 OK\r\n
            string strStatus = "";// response.Substring(" ", "\r\n");

            int simPos = strStatus.IndexOf(' ');

            if (simPos == -1)
            {
                //throw NewProxyException(Resources.ProxyException_ReceivedWrongResponse);
            }

            string statusLine = strStatus.Substring(0, simPos);

            if (statusLine.Length == 0)
            {
                //throw NewProxyException(Resources.ProxyException_ReceivedWrongResponse);
            }

            HttpStatusCode statusCode = (HttpStatusCode)Enum.Parse(
                typeof(HttpStatusCode), statusLine);

            return statusCode;
        }

        private void WaitData(NetworkStream nStream)
        {
            int sleepTime = 0;
            int delay = (nStream.ReadTimeout < 10) ?
                10 : nStream.ReadTimeout;

            while (!nStream.DataAvailable)
            {
                if (sleepTime >= delay)
                {
                    //throw NewProxyException(Resources.ProxyException_WaitDataTimeout);
                }

                sleepTime += 10;
                Thread.Sleep(10);
            }
        }

        #endregion
    }
}
