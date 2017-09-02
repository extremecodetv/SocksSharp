# SocksSharp

![SocksSharp](http://i.imgur.com/hh1aZVU.png)


[![AppVeyor](https://img.shields.io/appveyor/ci/gruntjs/grunt/master.svg?style=flat-square)](https://ci.appveyor.com/project/extremecodetv/sockssharp/)
[![NuGet](https://img.shields.io/nuget/v/sockssharp.svg?style=flat-square)](https://www.nuget.org/packages/SocksSharp/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg?style=flat-square)](https://raw.githubusercontent.com/extremecodetv/SocksSharp/master/LICENSE)


SocksSharp provides support for Socks4/4a/5 proxy servers to [HttpClient](https://msdn.microsoft.com/en-us/library/system.net.http.httpclient(v=vs.118).aspx)

## Installation

Install as [NuGet package](https://www.nuget.org/packages/SocksSharp/):

```powershell
Install-Package SocksSharp
```

.NET CLI:

```shell
dotnet add package SocksSharp
```


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

Interesting? [See more](https://github.com/extremecodetv/SocksSharp/wiki)

## Contributing

Feel free to [open an issue](https://github.com/extremecodetv/SocksSharp/issues) or submit a [pull request](https://github.com/extremecodetv/SocksSharp/pulls). To make sure your pull request doesn't go in vain (gets declined), open an issue first discussing it (before actually implementing it).
