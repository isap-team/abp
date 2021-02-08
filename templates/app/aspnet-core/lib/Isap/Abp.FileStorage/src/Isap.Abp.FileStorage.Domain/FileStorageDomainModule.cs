using Isap.Abp.Extensions.MultiTenancy;
using Isap.Abp.FileStorage.Configurations;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Domain;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;

namespace Isap.Abp.FileStorage
{
	[DependsOn(
		typeof(AbpDddDomainModule),
		typeof(FileStorageDomainSharedModule)
	)]
	public class FileStorageDomainModule: AbpModule
	{
		public override void ConfigureServices(ServiceConfigurationContext context)
		{
			Configure<AbpMultiTenancyOptions>(options =>
				{
					options.IsEnabled = IsapMultiTenancyConsts.IsEnabled;
				});

			context.Services.AddSingleton<IFileStorageProviderFactory, FileStorageProviderFactory>();
		}
	}
}
