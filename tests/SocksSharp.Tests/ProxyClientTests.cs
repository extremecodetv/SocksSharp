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
using System.Collections.Generic;
using System.Net;

namespace SocksSharp.Tests
{
    public class ProxyClientTests
    {
        private ProxySettings proxySettings;

        public ProxyClientTests()
        {
            GatherTestConfiguration();
        }

        #region Tests

        [Fact]
        public async Task RequestHeadersTest()
        {
            EnsureIsConfigured();

            var userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.113 Safari/537.36";

            var message = new HttpRequestMessage();
            message.Method = HttpMethod.Get;
            message.RequestUri = new Uri("http://httpbin.org/user-agent");
            message.Headers.Add("User-Agent", userAgent);

            var response = await GetResponseMessageAsync(message);

            Assert.NotNull(response);

            var userAgentActual = await GetJsonStringValue(response, "user-agent");

            Assert.NotEmpty(userAgentActual);
            Assert.Equal(userAgent, userAgentActual);
        }

        [Fact]
        public async Task GetRequestTest()
        {
            EnsureIsConfigured();

            var key = "key";
            var value = "value";

            var message = new HttpRequestMessage();
            message.Method = HttpMethod.Get;
            message.RequestUri = new Uri($"http://httpbin.org/get?{key}={value}");

            var response = await GetResponseMessageAsync(message);

            var actual = await GetJsonDictionaryValue(response, "args");

            Assert.True(actual.ContainsKey(key));
            Assert.True(actual.ContainsValue(value));
        }

        [Fact]
        public async Task GetUtf8Test()
        {
            EnsureIsConfigured();

            var excepted = "∮";

            var message = new HttpRequestMessage();
            message.Method = HttpMethod.Get;
            message.RequestUri = new Uri("http://httpbin.org/encoding/utf8");


            var response = await GetResponseMessageAsync(message);
            var actual = await response.Content.ReadAsStringAsync();

            Assert.Contains(excepted, actual);
        }

        [Fact]
        public async Task GetHtmlPageTest()
        {
            EnsureIsConfigured();

            long exceptedLength = 3741;
            var contentType = "text/html";
            var charSet = "utf-8";

            var message = new HttpRequestMessage();
            message.Method = HttpMethod.Get;
            message.RequestUri = new Uri("http://httpbin.org/html");

            var response = await GetResponseMessageAsync(message);

            var content = response.Content;
            Assert.NotNull(content);

            var headers = response.Content.Headers;
            Assert.NotNull(headers);

            Assert.NotNull(headers.ContentLength);
            Assert.Equal(exceptedLength, headers.ContentLength.Value);
            Assert.NotNull(headers.ContentType);
            Assert.Equal(contentType, headers.ContentType.MediaType);
            Assert.Equal(charSet, headers.ContentType.CharSet);
        }

        [Fact]
        public async Task DelayTest()
        {
            EnsureIsConfigured();

            var message = new HttpRequestMessage();
            message.Method = HttpMethod.Get;
            message.RequestUri = new Uri("http://httpbin.org/delay/4");

            var response = await GetResponseMessageAsync(message);
            var source = response.Content.ReadAsStringAsync();

            Assert.NotNull(response);
            Assert.NotNull(source);
        }

        [Fact]
        public async Task StreamTest()
        {
            EnsureIsConfigured();

            var message = new HttpRequestMessage();
            message.Method = HttpMethod.Get;
            message.RequestUri = new Uri("http://httpbin.org/stream/20");

            var response = await GetResponseMessageAsync(message);
            var source = response.Content.ReadAsStringAsync();

            Assert.NotNull(response);
            Assert.NotNull(source);
        }

        [Fact]
        public async Task GzipTest()
        {
            EnsureIsConfigured();

            var excepted = "gzip, deflate";

            var message = new HttpRequestMessage();
            message.Method = HttpMethod.Get;
            message.RequestUri = new Uri("http://httpbin.org/gzip");

            var response = await GetResponseMessageAsync(message);
            var acutal = await GetJsonStringValue(response, "Accept-Encoding");

            Assert.NotNull(response);
            Assert.NotNull(acutal);
            Assert.Equal(excepted, acutal);
        }

        [Fact]
        public async Task CookiesTest()
        {
            EnsureIsConfigured();

            HttpResponseMessage response = null;

            var name = "name";
            var value = "value";

            var message = new HttpRequestMessage();
            message.Method = HttpMethod.Get;
            message.RequestUri = new Uri($"http://httpbin.org/cookies/set?{name}={value}");

            var handler = CreateNewSocks5Client();
            handler.CookieContainer = new System.Net.CookieContainer();
            var client = new HttpClient(handler);

            try
            {
                response = await client.SendAsync(message);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception caught! " + ex.Message);
            }

            Assert.NotNull(response);
            var cookies = handler.CookieContainer.GetCookies(new Uri("http://httpbin.org/"));

            Assert.Equal(1, cookies.Count);
            var cookie = cookies[name];
            Assert.Equal(name, cookie.Name);
            Assert.Equal(value, cookie.Value);

            handler.Dispose();
            client.Dispose();
        }

        [Fact]
        public async Task StatusCodeTest()
        {
            EnsureIsConfigured();

            var code = "404";
            var excepted = "NotFound";
            var message = new HttpRequestMessage();
            message.Method = HttpMethod.Get;
            message.RequestUri = new Uri($"http://httpbin.org/status/{code}");

            var response = await GetResponseMessageAsync(message);

            Assert.NotNull(response);
            Assert.Equal(excepted, response.StatusCode.ToString());
        }

        [Fact]
        public async Task HttpClient_SendAsync_SetExplicitHostHeader_ShouldNotFail()
        {
            var message = new HttpRequestMessage(HttpMethod.Get, "https://httpbin.org/headers");
            message.Headers.Host = "httpbin.org";

            var response = await GetResponseMessageAsync(message).ConfigureAwait(false);

            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        #endregion

        #region Helpers

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
            catch (FileNotFoundException)
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

        private async Task<string> GetJsonStringValue(HttpResponseMessage response, string valueName)
        {
            JToken token;
            var source = await response.Content.ReadAsStringAsync();
            var obj = JObject.Parse(source);

            var result = obj.TryGetValue(valueName, out token);

            if (!result)
            {
                return String.Empty;
            }

            return token.Value<string>();
        }

        private async Task<Dictionary<string, string>> GetJsonDictionaryValue(HttpResponseMessage response, string valueName)
        {
            JToken token;
            var source = await response.Content.ReadAsStringAsync();
            var obj = JObject.Parse(source);

            var result = obj.TryGetValue(valueName, out token);

            if (!result)
            {
                return null;
            }

            return token.ToObject<Dictionary<string, string>>();
        }

        private async Task<HttpResponseMessage> GetResponseMessageAsync(HttpRequestMessage requestMessage)
        {
            HttpResponseMessage response = null;

            var handler = CreateNewSocks5Client();
            var client = new HttpClient(handler);

            try
            {
                response = await client.SendAsync(requestMessage);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception caught! " + ex.Message);
            }

            handler.Dispose();
            client.Dispose();

            return response;
        }

        private void EnsureIsConfigured()
        {
            if (proxySettings == null
                || proxySettings.Host == null
                || proxySettings.Port == 0)
            {
                throw new Exception("Please add your proxy settings to proxysettings.json in build folder");
            }
        }

        #endregion
    }
}
