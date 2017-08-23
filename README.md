# SocksSharp

[![AppVeyor](https://img.shields.io/appveyor/ci/gruntjs/grunt/master.svg?style=flat-square)](https://ci.appveyor.com/project/extremecodetv/sockssharp/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg?style=flat-square)](https://raw.githubusercontent.com/extremecodetv/SocksSharp/master/LICENSE)

![SocksSharp](http://i.imgur.com/hh1aZVU.png)


SocksSharp provides support for Socks4/4a/5 proxy servers to [HttpClient](https://msdn.microsoft.com/en-us/library/system.net.http.httpclient(v=vs.118).aspx)

## Basic Usage
```C#
var settings = new ProxySettings()
{
	Host = "127.0.0.1",
	Port = 1080
};

using (var proxyClientHandler = new ProxyClientHandler<Socks5>(settings))
{
	using (var httpClient = new HttpClient(proxyClientHandler))
	{
		var response = await httpClient.GetAsync("http://example.com/");
	}
}
```

## Installation

Install as [NuGet package](https://www.nuget.org/packages/SocksSharp/):

```powershell
Install-Package SocksSharp
```
