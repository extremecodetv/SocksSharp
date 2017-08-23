using System;
using System.Text;
using System.Net.Http;
using System.Collections.Generic;

using SocksSharp.Extensions;
using SocksSharp.Helpers;
using System.Net.Http.Headers;

namespace SocksSharp.Proxy.Request
{
    internal class RequestBuilder
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

        public byte[] BuildHeaders(bool hasContent)
        {
            var headers = GetHeaders(request.Headers);
            if (hasContent)
            {
                var contentHeaders = GetHeaders(request.Content.Headers);
                headers = String.Join(newLine, headers, contentHeaders);
            }

            return ToByteArray(headers + newLine + newLine);
        }
        
        private string GetHeaders(HttpHeaders headers)
        {
            var headersList = new List<string>();

            foreach (var header in headers)
            {
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
                        headerKeyAndValue = header.Key + ": " + headerValue;
                    }
                }

                if (!String.IsNullOrEmpty(headerKeyAndValue))
                {
                    headersList.Add(headerKeyAndValue);
                }
            }

            if (headers is HttpContentHeaders && !headersList.Contains("Content-Length"))
            {
                var content = headers as HttpContentHeaders;
                if(content.ContentLength.HasValue && content.ContentLength.Value > 0)
                {
                    headersList.Add($"Content-Length: {content.ContentLength}");
                }
            }

            return String.Join("\r\n", headersList.ToArray());
        }

        private byte[] ToByteArray(string data)
        {
            return Encoding.ASCII.GetBytes(data);
        }
    }
}
