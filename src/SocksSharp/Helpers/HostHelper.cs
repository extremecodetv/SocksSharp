using System;
using System.Collections.Generic;
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
    }
}
