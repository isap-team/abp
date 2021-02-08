using Isap.Abp.Extensions;
using Isap.CommonCore.Services;
using Volo.Abp.Modularity;

namespace Isap.Abp.FileStorage
{
	[DependsOn(
		typeof(IsapAbpFileStorageApplicationContractsModule),
		typeof(IsapAbpExtensionsHttpApiClientModule))]
	public class FileStorageHttpApiClientModule: AbpModule
	{
		public const string RemoteServiceName = "FileStorage";

		public override void ConfigureServices(ServiceConfigurationContext context)
		{
			context.Services.AddHttpClientProxies<ICommonAppService>(
				typeof(IsapAbpFileStorageApplicationContractsModule).Assembly,
				RemoteServiceName
			);
		}
	}
}
