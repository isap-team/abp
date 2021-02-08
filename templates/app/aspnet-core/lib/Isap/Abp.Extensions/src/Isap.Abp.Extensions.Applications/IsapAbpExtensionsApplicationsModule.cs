using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AutoMapper;
using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace Isap.Abp.Extensions
{
	[DependsOn(
		typeof(IsapAbpExtensionsModule),
		typeof(AbpAspNetCoreMvcModule),
		typeof(AbpDddDomainModule))]
	public class IsapAbpExtensionsApplicationsModule: AbpModule
	{
		public override void ConfigureServices(ServiceConfigurationContext context)
		{
			Configure<AbpAutoMapperOptions>(options =>
				{
					options.AddMaps<IsapAbpExtensionsApplicationsModule>(validate: true);
				});
		}
	}
}
