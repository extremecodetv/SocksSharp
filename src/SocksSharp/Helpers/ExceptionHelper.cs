
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
