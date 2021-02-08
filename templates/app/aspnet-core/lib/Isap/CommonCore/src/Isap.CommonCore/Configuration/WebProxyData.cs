using System;
using System.Net;

namespace Isap.CommonCore.Configuration
{
	public class WebProxyData: IWebProxy
	{
		public WebProxyData(IProxyConfiguration config)
		{
			Config = config;
			Credentials = config.Credentials;
		}

		public IProxyConfiguration Config { get; }

		public ICredentials Credentials { get; set; }

		public Uri GetProxy(Uri destination)
		{
			return Config.ProxyUrl;
		}

		public bool IsBypassed(Uri host)
		{
			return false;
		}
	}
}
