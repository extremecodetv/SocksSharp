using System;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace SocksSharp.Tests
{
    public class ProxyClientTests
    {

        private void GatherTestConfiguration()
        {
            var appConfigMsgWarning = "{0} not configured in proxysettings.json! Some tests may fail.";

            var builder = new ConfigurationBuilder()
                .AddJsonFile("proxysettings.json")
                .Build();
        }

        [Fact]
        public void Test1()
        {

        }
    }
}
