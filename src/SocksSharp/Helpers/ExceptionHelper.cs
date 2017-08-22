using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocksSharp.Helpers
{
    internal static class ExceptionHelper
    {
        public static bool ValidateTcpPort(int port)
        {
            if (port < 1 || port > 65535)
            {
                return false;
            }

            return true;
        }
    }
}
