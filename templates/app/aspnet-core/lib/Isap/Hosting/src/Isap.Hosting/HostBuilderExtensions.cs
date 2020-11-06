using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Autofac;
using Isap.CommonCore.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Volo.Abp.Autofac;

namespace Isap.Hosting
{
	public static class HostBuilderExtensions
	{
		public static async Task BuildAndRunAsync(this IHostBuilder hostBuilder)
		{
			LoggingContext.SetDefault(new LoggingContext(new SerilogLoggerContextPropertyFactory()));

			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				if (Environment.UserInteractive)
					hostBuilder.UseConsoleLifetime();
				else
					hostBuilder.UseWindowsService();
			}
			else //if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
			{
				if (Environment.UserInteractive)
					hostBuilder.UseConsoleLifetime();
				else
					hostBuilder.UseSystemd();
			}

			IHost host = hostBuilder.Build();
			IHostLifetime hostLifetime = host.Services.GetRequiredService<IHostLifetime>();
			if (hostLifetime is IDisposable disposableLifetime)
			{
				using (disposableLifetime)
				{
					await host.RunAsync();
				}
			}
			else
			{
				await host.RunAsync();
			}
		}

		public static void BuildAndRun(this IHostBuilder hostBuilder)
		{
			hostBuilder.BuildAndRunAsync().GetAwaiter().GetResult();
		}

		public static IHostBuilder UseAutofac(this IHostBuilder hostBuilder, Action<ContainerBuilder> prepare)
		{
			var containerBuilder = new ContainerBuilder();

			prepare(containerBuilder);

			return hostBuilder.ConfigureServices((_, services) =>
					{
						services.AddObjectAccessor(containerBuilder);
					})
				.UseServiceProviderFactory(new AbpAutofacServiceProviderFactory(containerBuilder));
		}
	}
}
