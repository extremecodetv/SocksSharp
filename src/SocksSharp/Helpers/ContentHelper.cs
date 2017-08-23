using System;

namespace SocksSharp.Helpers
{
    internal static class ContentHelper
    {
        public static bool IsContentHeader(string name)
        {
            //https://github.com/dotnet/corefx/blob/3e72ee5971db5d0bd46606fa672969adde29e307/src/System.Net.Http/src/System/Net/Http/Headers/KnownHeaders.cs
            var contentHeaders = new string[]
            {
                "Last-Modified",
                "Expires",
                "Content-Type",
                "Content-Range",
                "Content-MD5",
                "Content-Location",
                "Content-Length",
                "Content-Language",
                "Content-Encoding",
                "Allow"
            };

            bool isContent = false;
            foreach(var header in contentHeaders)
            {
                isContent = isContent || header.Equals(name, StringComparison.OrdinalIgnoreCase);
            }

            return isContent;
        }
    }
}
