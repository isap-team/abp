using System;
using System.Collections.Generic;
using Isap.CommonCore.Configuration;
using Isap.CommonCore.Utils;

namespace Isap.CommonCore.Integrations
{
	public interface IIntegrationsConfiguration
	{
		TimeZoneIdSystemType ServerTimeZoneIdSystem { get; }
		IProxyConfiguration Proxy { get; }
		IDictionary<string, IApiAgentConfiguration> Agents { get; }

		T GetValue<T>(string key, Func<T> getDefaultValue);
		T GetValue<T>(string key, T defaultValue = default(T));
	}
}
