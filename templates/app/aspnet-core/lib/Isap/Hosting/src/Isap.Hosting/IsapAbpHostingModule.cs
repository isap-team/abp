using Isap.CommonCore.Logging;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace Isap.Hosting
{
	[DependsOn(
		typeof(AbpAutofacModule)
	)]
	public class IsapAbpHostingModule: AbpModule
	{
		public override void ConfigureServices(ServiceConfigurationContext context)
		{
			base.ConfigureServices(context);

			context.Services.AddTransient(_ => LoggingContext.Current);
		}
	}
}
