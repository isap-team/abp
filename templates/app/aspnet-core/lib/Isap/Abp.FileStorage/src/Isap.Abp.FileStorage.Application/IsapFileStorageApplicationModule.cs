using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Application;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace Isap.Abp.FileStorage
{
	[DependsOn(
		typeof(FileStorageDomainModule),
		typeof(IsapAbpFileStorageApplicationContractsModule),
		typeof(AbpDddApplicationModule),
		typeof(AbpAutoMapperModule)
	)]
	public class IsapFileStorageApplicationModule: AbpModule
	{
		public override void ConfigureServices(ServiceConfigurationContext context)
		{
			context.Services.AddAutoMapperObjectMapper<IsapFileStorageApplicationModule>();
			Configure<AbpAutoMapperOptions>(options =>
				{
					options.AddMaps<IsapFileStorageApplicationModule>(validate: true);
				});
		}
	}
}
