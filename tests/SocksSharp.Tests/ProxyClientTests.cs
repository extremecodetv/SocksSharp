using System;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Xunit;

using SocksSharp;
using SocksSharp.Proxy;

namespace SocksSharp.Tests
{
    public class ProxyClientTests
    {
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

        [Fact]
        public void Test1()
        {

        }
    }
}
