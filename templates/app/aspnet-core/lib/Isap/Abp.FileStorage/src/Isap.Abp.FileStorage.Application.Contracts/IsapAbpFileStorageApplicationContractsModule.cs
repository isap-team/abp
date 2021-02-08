using Volo.Abp.Application;
using Volo.Abp.Authorization;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace Isap.Abp.FileStorage
{
	[DependsOn(
		typeof(FileStorageDomainSharedModule),
		typeof(AbpDddApplicationContractsModule),
		typeof(AbpAuthorizationModule)
	)]
	public class IsapAbpFileStorageApplicationContractsModule: AbpModule
	{
		public override void ConfigureServices(ServiceConfigurationContext context)
		{
			Configure<AbpVirtualFileSystemOptions>(options =>
				{
					options.FileSets.AddEmbedded<IsapAbpFileStorageApplicationContractsModule>("Isap.Abp.FileStorage");
				});
		}
	}
}
