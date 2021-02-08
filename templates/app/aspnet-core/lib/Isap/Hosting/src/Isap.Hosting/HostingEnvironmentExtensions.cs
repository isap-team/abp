using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Isap.Hosting
{
	public static class HostingEnvironmentExtensions
	{
		public static IConfigurationRoot GetAppConfiguration(this IHostEnvironment env, Action<IConfigurationBuilder> addUserSecrets = null)
		{
			return AppConfigurations.Get(env.ContentRootPath, env.EnvironmentName, builder =>
				{
					builder
						.AddJsonFile("serilog.json", true)
						.AddJsonFile($"serilog.{env.EnvironmentName}.json", true)
						;
					if (env.IsDevelopment())
					{
						addUserSecrets?.Invoke(builder);
					}
				});
		}
	}
}
