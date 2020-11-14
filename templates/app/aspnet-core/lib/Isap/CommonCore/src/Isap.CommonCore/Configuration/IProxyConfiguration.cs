using System;
using System.Net;

namespace Isap.CommonCore.Configuration
{
	public interface IProxyConfiguration
	{
		bool UseProxy { get; }
		bool UseSocks5 { get; }
		Uri ProxyUrl { get; }
		NetworkCredential Credentials { get; }

		IWebProxy GetWebProxy();
	}
}
