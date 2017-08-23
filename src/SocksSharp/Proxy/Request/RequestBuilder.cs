using SocksSharp.Extensions;
using SocksSharp.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SocksSharp.Proxy.Request
{
    internal class RequestBuilder : IRequestBuilder
    {
        private readonly string newLine = "\r\n";

        private HttpRequestMessage request;

        public RequestBuilder(HttpRequestMessage request)
        {
            this.request = request;
        }

        public byte[] BuildStartingLine()
        {
            var uri = request.RequestUri;

            string startingLine
                    = $"{request.Method.Method} {uri.PathAndQuery} HTTP/{request.Version.ToString()}" + newLine;

            startingLine += "Host: " + uri.Host + newLine;

            return ToByteArray(startingLine);
        }

        public byte[] BuildHeaders()
        {
            var headersList = new List<string>();
            var headers = request.Headers;

            foreach (var header in headers)
            {
                if (ContentHelper.IsContentHeader(header.Key))
                {
                    continue;
                }

                string headerKeyAndValue = String.Empty;
                string[] values = header.Value as string[];

                if (values != null && values.Length < 2)
                {
                    if (values.Length > 0 && !String.IsNullOrEmpty(values[0]))
                    {
                        headerKeyAndValue = header.Key + ": " + values[0];
                    }
                }
                else
                {
                    string headerValue = headers.GetHeaderString(header.Key);
                    if (!String.IsNullOrEmpty(headerValue))
                    {
                        headerKeyAndValue = header.Key + ": " + values[0];
                    }
                }

                if (!String.IsNullOrEmpty(headerKeyAndValue))
                {
                    headersList.Add(headerKeyAndValue);
                }
            }

            var rawHeaders = String.Join("\r\n", headersList.ToArray());

            return ToByteArray(rawHeaders + newLine);
        }
        
        private byte[] ToByteArray(string data)
        {
            return Encoding.ASCII.GetBytes(data);
        }
    }
}
