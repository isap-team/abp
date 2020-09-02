using System.Collections.Generic;
using System.Collections.ObjectModel;
using Isap.CommonCore.Configuration;
using Isap.CommonCore.Extensions;
using Isap.CommonCore.Utils;
using Isap.Converters;
using Microsoft.Extensions.Configuration;

namespace Isap.CommonCore.Integrations
{
	public class IntegrationsConfiguration: CustomSectionConfigurationBase, IIntegrationsConfiguration
	{
		private readonly IDictionary<string, IApiAgentConfiguration> _agents = new Dictionary<string, IApiAgentConfiguration>();

		public IntegrationsConfiguration(IValueConverter converter, IConfigurationSection config)
			: base(converter, config)
		{
			if (config != null)
			{
				ServerTimeZoneIdSystem = config[nameof(ServerTimeZoneIdSystem)]
						.IfNotNullThen(value => converter.TryConvertTo<TimeZoneIdSystemType>(value).AsDefaultIfNotSuccess(TimeZoneIdSystemType.Windows))
						.Else(TimeZoneIdSystemType.Windows)
					;
				Proxy = ProxyConfiguration.Create(converter, config.GetSection(nameof(Proxy)));
				IConfigurationSection agentsConfig = config.GetSection(nameof(Agents));
				if (agentsConfig != null)
				{
					foreach (IConfigurationSection agentConfig in agentsConfig.GetChildren())
					{
						IApiAgentConfiguration agent = ApiAgentConfiguration.Create(converter, agentConfig, Proxy);
						_agents.Add(agent.Key, agent);
					}
				}
			}
		}

		public TimeZoneIdSystemType ServerTimeZoneIdSystem { get; }
		public IProxyConfiguration Proxy { get; }

		public IDictionary<string, IApiAgentConfiguration> Agents => new ReadOnlyDictionary<string, IApiAgentConfiguration>(_agents);
	}
}
