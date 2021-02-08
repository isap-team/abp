using System.Net;
using Isap.CommonCore.Configuration;
using Isap.CommonCore.Utils;

namespace Isap.CommonCore.Integrations
{
	public interface IApiAgentConfiguration
	{
		string Key { get; }
		IProxyConfiguration Proxy { get; }
		string BaseUrl { get; }
		NetworkCredential Credentials { get; }
		TimeZoneIdSystemType TimeZoneIdSystem { get; }
		bool IgnoreSslErrors { get; }

		IConfigValueProvider Config { get; }
	}
}
