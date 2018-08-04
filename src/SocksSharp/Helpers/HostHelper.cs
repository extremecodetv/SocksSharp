using SocksSharp.Proxy;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SocksSharp.Core.Helpers
{
    internal static class HostHelper
    {
        public static byte[] GetPortBytes(int port)
        {
            byte[] array = new byte[2];

            array[0] = (byte)(port / 256);
            array[1] = (byte)(port % 256);

            return array;
        }
        
        public static byte[] GetIPAddressBytes(string destinationHost,bool preferIpv4=true)
        {
            if (!IPAddress.TryParse(destinationHost, out var ipAddr))
            {
                try
                {
                    var ips = Dns.GetHostAddresses(destinationHost);

                    if (ips.Length > 0)
                    {
                        if (preferIpv4)
                        {
                            foreach (var ip in ips)
                            {
                                var ipBytes = ip.GetAddressBytes();
                                if (ipBytes.Length == 4)
                                {
                                    return ipBytes;
                                }
                            }
                        }

                        ipAddr = ips[0];
                    }
                }
                catch (Exception ex)
                {
                    if (ex is SocketException || ex is ArgumentException)
                    {
                        throw new ProxyException("Failed to get host address", ex);
                    }

                    throw;
                }
            }

            return ipAddr.GetAddressBytes();
        }

        public static byte[] GetHostAddressBytes(byte addressType, string host)
        {
            switch (addressType)
            {
                case Socks5Constants.AddressTypeIPV4:
                case Socks5Constants.AddressTypeIPV6:
                    return IPAddress.Parse(host).GetAddressBytes();

                case Socks5Constants.AddressTypeDomainName:
                    byte[] bytes = new byte[host.Length + 1];

                    bytes[0] = (byte)host.Length;
                    Encoding.ASCII.GetBytes(host).CopyTo(bytes, 1);

                    return bytes;

                default:
                    return null;
            }
        }
    }
}
