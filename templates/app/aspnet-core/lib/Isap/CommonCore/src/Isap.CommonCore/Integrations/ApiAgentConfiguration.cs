using System.Net;
using Isap.CommonCore.Configuration;
using Isap.CommonCore.Extensions;
using Isap.CommonCore.Utils;
using Isap.Converters;
using Microsoft.Extensions.Configuration;

namespace Isap.CommonCore.Integrations
{
	public class ApiAgentConfiguration: IApiAgentConfiguration
	{
		public ApiAgentConfiguration(string key, string baseUrl)
		{
			Key = key;
			BaseUrl = baseUrl;
			Credentials = null;
			TimeZoneIdSystem = TimeZoneIdSystemType.Windows;
			IgnoreSslErrors = false;
		}

		public string Key { get; }
		public string BaseUrl { get; }
		public NetworkCredential Credentials { get; set; }
		public TimeZoneIdSystemType TimeZoneIdSystem { get; set; }
		public bool IgnoreSslErrors { get; set; }

		public IProxyConfiguration Proxy { get; set; }
		public IConfigValueProvider Config { get; set; }

		public static ApiAgentConfiguration Create(IValueConverter converter, IConfigurationSection agentConfig, IProxyConfiguration defaultProxyConfig)
		{
			string baseUrl = agentConfig[nameof(BaseUrl)];
			NetworkCredential credentials = agentConfig.GetSection(nameof(Credentials)).LoadCredentials();

			// ReSharper disable once RedundantArgumentDefaultValue
			TimeZoneIdSystemType timeZoneIdSystem = agentConfig[nameof(TimeZoneIdSystem)]
					.IfNotNullThen(value => converter.TryConvertTo<TimeZoneIdSystemType>(value).AsDefaultIfNotSuccess(TimeZoneIdSystemType.Windows))
					.Else(TimeZoneIdSystemType.Windows)
				;

			bool ignoreSslErrors = agentConfig[nameof(IgnoreSslErrors)].IfNullThen(false).Else(v => converter.TryConvertTo<bool>(v).AsDefaultIfNotSuccess());

			IConfigurationSection proxyConfig = agentConfig.GetSection(nameof(Proxy));
			IProxyConfiguration proxy = proxyConfig == null ? null : ProxyConfiguration.Create(converter, proxyConfig, defaultProxyConfig);

			return new ApiAgentConfiguration(agentConfig.Key, baseUrl)
				{
					Credentials = credentials,
					TimeZoneIdSystem = timeZoneIdSystem,
					IgnoreSslErrors = ignoreSslErrors,
					Proxy = proxy,
					Config = new ConfigurationSectionValueProvider(converter, agentConfig),
				};
		}
	}
}
