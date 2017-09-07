using System;
using System.Diagnostics;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Xunit;

using SocksSharp;
using SocksSharp.Proxy;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SocksSharp.Tests
{
    public class ProxyClientTests
    {
        private ProxySettings proxySettings;

        private void GatherTestConfiguration()
        {
            IConfigurationRoot configuration;

            var appConfigMsgWarning = "{0} not configured in proxysettings.json! Some tests may fail.";


            try
            {
                configuration = new ConfigurationBuilder()
                                .AddJsonFile("proxysettings.json")
                                .Build();
            }
            catch(FileNotFoundException)
            {
                Debug.WriteLine("proxysettings.json not found in project folder");
                return;
            }

            proxySettings = new ProxySettings();

            var host = configuration["host"];
            if (String.IsNullOrEmpty(host))
            {
                Debug.WriteLine(String.Format(appConfigMsgWarning, nameof(host)));
            }
            else
            {
                proxySettings.Host = host;
            }

            var port = configuration["port"];
            if (String.IsNullOrEmpty(port))
            {
                Debug.WriteLine(String.Format(appConfigMsgWarning, nameof(port)));
            }
            else
            {
                proxySettings.Port = Int32.Parse(port);
            }

            //TODO: Setup manualy
            var username = configuration["username"];
            var password = configuration["password"];
        }

        private ProxyClientHandler<Socks5> CreateNewSocks5Client()
        {
            return new ProxyClientHandler<Socks5>(proxySettings);
        }

        public ProxyClientTests()
        {
            GatherTestConfiguration();
        }

        [Fact]
        public virtual async Task RequestHeadersTest()
        {
            CheckIsConfigured();

            var userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.113 Safari/537.36";

            var message = new HttpRequestMessage();
            message.Method = HttpMethod.Get;
            message.RequestUri = new Uri("http://httpbin.org/user-agent");
            message.Headers.Add("User-Agent", userAgent);

            var response = await GetResponseMessage(message);

            Assert.NotNull(response);

            var obj = await Deserialize(response);
            Assert.Equal(obj.user-agent, userAgent);
        }

        private async Task<dynamic> Deserialize(HttpResponseMessage response)
        {
            var source = await response.Content.ReadAsStringAsync();
            return JObject.Parse(source);
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

        private void CheckIsConfigured()
        {
            if (proxySettings == null 
                || proxySettings.Host == null 
                || proxySettings.Port == 0)
            {
                throw new Exception("Please add your proxy settings to proxysettings.json in build folder");
            }            
        }
    }
}
