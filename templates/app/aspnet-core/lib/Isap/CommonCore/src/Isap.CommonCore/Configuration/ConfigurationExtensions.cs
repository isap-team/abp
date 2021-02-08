using System;
using System.Net;
using Isap.CommonCore.Extensions;
using Isap.CommonCore.Integrations;
using Microsoft.Extensions.Configuration;

namespace Isap.CommonCore.Configuration
{
	public static class ConfigurationExtensions
	{
		public static NetworkCredential LoadCredentials(this IConfigurationSection credentialsConfig, NetworkCredential parentCredentials = null)
		{
			if (credentialsConfig == null)
				return parentCredentials /* ?? CredentialCache.DefaultNetworkCredentials*/;
			string userName = credentialsConfig[nameof(NetworkCredential.UserName)]
						.IfNullThen(parentCredentials?.UserName)
						.Else(value => value)
					?? String.Empty
				;
			string password = credentialsConfig[nameof(NetworkCredential.Password)]
						.IfNullThen(parentCredentials?.Password)
						.Else(value => value)
					?? String.Empty
				;
			return String.IsNullOrEmpty(userName)
					? null //CredentialCache.DefaultNetworkCredentials
					: new NetworkCredential(userName, password)
				;
		}

		public static NetworkCredential LoadCredentials(this IConfigValueProvider credentialsConfig, NetworkCredential parentCredentials = null)
		{
			if (credentialsConfig == null)
				return parentCredentials /* ?? CredentialCache.DefaultNetworkCredentials*/;
			string userName = credentialsConfig.GetValue<string>(nameof(NetworkCredential.UserName))
						.IfNullThen(parentCredentials?.UserName)
						.Else(value => value)
					?? String.Empty
				;
			string password = credentialsConfig.GetValue<string>(nameof(NetworkCredential.Password))
						.IfNullThen(parentCredentials?.Password)
						.Else(value => value)
					?? String.Empty
				;
			return String.IsNullOrEmpty(userName)
					? null //CredentialCache.DefaultNetworkCredentials
					: new NetworkCredential(userName, password)
				;
		}
	}
}
