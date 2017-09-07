using System;
using System.Diagnostics;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Xunit;

using SocksSharp;
using SocksSharp.Proxy;
using System.Threading.Tasks;

namespace SocksSharp.Tests
{
    public class ProxyClientTests
    {
        private Uri baseUri = new Uri("http://httpbin.org/");
        private ProxySettings proxySettings;

        private void GatherTestConfiguration()
        {
            var appConfigMsgWarning = "{0} not configured in proxysettings.json! Some tests may fail.";

            var builder = new ConfigurationBuilder()
                .AddJsonFile("proxysettings.json")
                .Build();

            proxySettings = new ProxySettings();

            var host = builder["host"];
            if (String.IsNullOrEmpty(host))
            {
                Debug.WriteLine(String.Format(appConfigMsgWarning, nameof(host)));
            }
            else
            {
                proxySettings.Host = host;
            }

            var port = builder["port"];
            if (String.IsNullOrEmpty(port))
            {
                Debug.WriteLine(String.Format(appConfigMsgWarning, nameof(port)));
            }
            else
            {
                proxySettings.Port = Int32.Parse(port);
            }

            //TODO: Setup manualy
            var username = builder["username"];
            var password = builder["password"];
        }

        private ProxyClientHandler<Socks5> CreateNewSocks5Client()
        {
            if(proxySettings.Host == null || proxySettings.Port == 0)
            {
                throw new Exception("Please add your proxy settings to proxysettings.json!");
            }

            return new ProxyClientHandler<Socks5>(proxySettings);
        }

        public ProxyClientTests()
        {
            GatherTestConfiguration();

        }

        [Fact]
        public async Task TestRequestHeaders()
        {
            var userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.113 Safari/537.36";

            var message = new HttpRequestMessage();
            message.Method = HttpMethod.Get;
            message.RequestUri = new Uri("http://httpbin.org/user-agent");
            message.Headers.Add("User-Agent", userAgent);

            var response = await GetResponseMessage(message);
        }

        private async Task<HttpResponseMessage> GetResponseMessage(HttpRequestMessage requestMessage)
        {
            HttpResponseMessage response = null;

            var handler = CreateNewSocks5Client();
            var client = new HttpClient(handler);

            try
            {
                response = await client.SendAsync(requestMessage);
            }
            catch(Exception ex)
            {
                Debug.WriteLine("Exception caught! " + ex.Message);
            }

            handler.Dispose();
            client.Dispose();

            return response;
        }
    }
}
