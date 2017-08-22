using System;
using System.Text;
using System.Net.Sockets;

namespace SocksSharp.Proxy
{
    public class Socks4a : Socks4
    {
        internal protected override void SendCommand(NetworkStream nStream, byte command, string destinationHost, int destinationPort)
        {
            byte[] dstPort = GetPortBytes(destinationPort);
            byte[] dstIp = { 0, 0, 0, 1 };

            byte[] userId = new byte[0];
            if (Settings.Credentials != null)
            {
                if (!String.IsNullOrEmpty(Settings.Credentials.UserName))
                {
                    userId = Encoding.ASCII.GetBytes(Settings.Credentials.UserName);
                }
            }

            byte[] dstAddr = Encoding.ASCII.GetBytes(destinationHost);

            // +----+----+----+----+----+----+----+----+----+----+....+----+----+----+....+----+
            // | VN | CD | DSTPORT |      DSTIP        | USERID       |NULL| DSTADDR      |NULL|
            // +----+----+----+----+----+----+----+----+----+----+....+----+----+----+....+----+
            //    1    1      2              4           variable       1    variable        1 
            byte[] request = new byte[10 + userId.Length + dstAddr.Length];

            request[0] = VersionNumber;
            request[1] = command;
            dstPort.CopyTo(request, 2);
            dstIp.CopyTo(request, 4);
            userId.CopyTo(request, 8);
            request[8 + userId.Length] = 0x00;
            dstAddr.CopyTo(request, 9 + userId.Length);
            request[9 + userId.Length + dstAddr.Length] = 0x00;

            nStream.Write(request, 0, request.Length);

            // +----+----+----+----+----+----+----+----+
            // | VN | CD | DSTPORT |      DSTIP        |
            // +----+----+----+----+----+----+----+----+
            //    1    1      2              4
            byte[] response = new byte[8];

            nStream.Read(response, 0, 8);

            byte reply = response[1];

            // Если запрос не выполнен.
            if (reply != CommandReplyRequestGranted)
            {
                HandleCommandError(reply);
            }
        }
    }
}
