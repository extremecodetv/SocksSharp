using SocksSharp.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SocksSharp.Helpers
{
    internal class RequestDataBuilder
    {
        private readonly string newLine = "\r\n";
        private List<byte> bytesToSend;

        public RequestDataBuilder()
        {
            bytesToSend = new List<byte>();
        }

        //public IRequestDataBuilder Add(string data)
        //{
        //    var buffer = Encoding.ASCII.GetBytes(data);
        //    bytesToSend.AddRange(buffer);
        //}

        public byte[] GetData()
        {
            return bytesToSend.ToArray();
        }

        public async Task WriteToAsync(Stream stream, CancellationToken ct)
        {
            var temp = bytesToSend.ToArray();
            var buffer = new byte[1024];
            var done = false;

            var raw = Encoding.Default.GetString(temp);
            int offset = 0;

            while (!done)
            {
                if(buffer.Length > (bytesToSend.Count - offset))
                {
                    buffer = new byte[bytesToSend.Count - offset - 1];
                    done = true;
                }

                buffer = bytesToSend.GetRange(offset, buffer.Length).ToArray();
                await stream.WriteAsync(buffer, offset, buffer.Length, ct);

                offset += buffer.Length;
            }

        }
        
        private string GetRequestHeaders(HttpHeaders headers)
        {
            var headersList = new List<string>();

            foreach (var header in headers)
            {
                if (String.Equals(header.Key, "Content-Length", StringComparison.OrdinalIgnoreCase))
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

            return String.Join("\r\n", headersList.ToArray());
        }

        private void AddRequestHeadersToBuilder(HttpHeaders headers)
        {
            var rawHeaders = GetRequestHeaders(headers);

            if (!String.IsNullOrEmpty(rawHeaders))
            {
                //requestDataBuilder.Add(rawHeaders);
            }
        }

        private void PrepareRequest(HttpRequestMessage request)
        {
            var uri = request.RequestUri;

            #region Starting Line
            string startingLine
                = $"{request.Method.Method} {uri.PathAndQuery} HTTP/{request.Version.ToString()}" + newLine;

            startingLine += "Host: " + uri.Host + newLine;

            //requestDataBuilder.Add(startingLine);
            #endregion

            #region Headers and Content Headers
            if (request.Content != null)
            {
                AddRequestHeadersToBuilder(request.Content.Headers);
            }

            AddRequestHeadersToBuilder(request.Headers);
            #endregion

        }
    }
}
