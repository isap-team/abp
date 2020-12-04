using Isap.CommonCore.Services;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;

namespace Isap.Abp.Extensions
{
	[DependsOn(
		typeof(AbpHttpClientModule))]
	public class IsapAbpExtensionsHttpApiClientModule: AbpModule
	{
		public const string RemoteServiceName = "Default";

		public override void ConfigureServices(ServiceConfigurationContext context)
		{
			context.Services.AddHttpClientProxies<ICommonAppService>(
				typeof(IsapAbpExtensionsModule).Assembly,
				RemoteServiceName
			);
		}
	}
}
