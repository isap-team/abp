using System;
using System.Net;
using Isap.CommonCore.Extensions;
using Isap.CommonCore.Integrations;
using Isap.Converters;
using Microsoft.Extensions.Configuration;
using MihaZupan;

namespace Isap.CommonCore.Configuration
{
	public class ProxyConfiguration: IProxyConfiguration
	{
		public static IProxyConfiguration NoProxy = new ProxyConfiguration(false, false, null, null);

		private ProxyConfiguration(bool useProxy, bool useSocks5, Uri proxyUrl, NetworkCredential credentials)
		{
			UseProxy = useProxy;
			UseSocks5 = useSocks5;
			ProxyUrl = proxyUrl;
			Credentials = credentials;
		}

		public bool UseProxy { get; set; }
		public bool UseSocks5 { get; }
		public Uri ProxyUrl { get; set; }
		public NetworkCredential Credentials { get; set; }

		public IWebProxy GetWebProxy()
		{
			if (UseProxy && UseSocks5)
			{
				return new HttpToSocks5Proxy(ProxyUrl.Host, ProxyUrl.Port, Credentials.UserName, Credentials.Password);
			}

			return UseProxy ? new WebProxyData(this) : WebRequest.DefaultWebProxy;
		}

		public static IProxyConfiguration Create(IValueConverter converter, IConfigurationSection section, IProxyConfiguration parentConfig = null)
		{
			if (section == null)
				return NoProxy;

			bool useProxy = section[nameof(UseProxy)]
				.IfNullThen(parentConfig?.UseProxy ?? false)
				.Else(value => converter.TryConvertTo<bool>(value).AsDefaultIfNotSuccess());
			bool useSocks5 = section[nameof(UseSocks5)]
				.IfNullThen(parentConfig?.UseSocks5 ?? false)
				.Else(value => converter.TryConvertTo<bool>(value).AsDefaultIfNotSuccess());
			Uri proxyUrl = section[nameof(ProxyUrl)]
				.IfNullThen(parentConfig?.ProxyUrl)
				.Else(value => new Uri(value, UriKind.Absolute));
			NetworkCredential credentials = section.GetSection(nameof(Credentials)).LoadCredentials(parentConfig?.Credentials);

			return new ProxyConfiguration(useProxy, useSocks5, proxyUrl, credentials);
		}

		public static IProxyConfiguration Create(IConfigValueProvider config, IProxyConfiguration parentConfig = null)
		{
			if (config == null)
				return NoProxy;

			bool useProxy = config.GetValue<bool>(nameof(UseProxy));
			bool useSocks5 = config.GetValue<bool>(nameof(UseSocks5));
			Uri proxyUrl = config.GetValue<string>(nameof(ProxyUrl))
				.IfNullThen(parentConfig?.ProxyUrl)
				.Else(value => new Uri(value, UriKind.Absolute));
			NetworkCredential credentials = config.GetValueProvider(nameof(Credentials)).LoadCredentials(parentConfig?.Credentials);

			return new ProxyConfiguration(useProxy, useSocks5, proxyUrl, credentials);
		}

		public static IProxyConfiguration Create(Uri proxyUrl, NetworkCredential credentials = null, bool useProxy = true, bool useSocks5 = false)
		{
			return new ProxyConfiguration(useProxy, useSocks5, proxyUrl, credentials);
		}

		public static IProxyConfiguration Create(string proxyUrl, NetworkCredential credentials = null, bool useProxy = true, bool useSocks5 = false)
		{
			return new ProxyConfiguration(useProxy, useSocks5, new Uri(proxyUrl, UriKind.Absolute), credentials);
		}
	}
}
