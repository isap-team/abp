using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Configuration;

namespace Isap.Hosting
{
	public static class AppConfigurations
	{
		private static readonly ConcurrentDictionary<string, IConfigurationRoot> _configurationCache;

		static AppConfigurations()
		{
			_configurationCache = new ConcurrentDictionary<string, IConfigurationRoot>();
		}

		public static IConfigurationRoot Get(string path, string environmentName = null, Action<IConfigurationBuilder> build = null)
		{
			environmentName ??= Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

			var cacheKey = path + "#" + environmentName + "#" + (build != null);
			return _configurationCache.GetOrAdd(
				cacheKey,
				_ => BuildConfiguration(path, environmentName, build)
			);
		}

		private static IConfigurationRoot BuildConfiguration(string path, string environmentName = null, Action<IConfigurationBuilder> addUserSecrets = null)
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(path)
				.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

			if (!environmentName.IsNullOrWhiteSpace())
			{
				builder = builder.AddJsonFile($"appsettings.{environmentName}.json", optional: true);
			}

			builder = builder.AddEnvironmentVariables();

			addUserSecrets?.Invoke(builder);

			return builder.Build();
		}
	}
}
