using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AspNetCore.MultiTenancy;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Modularity;

namespace Isap.Abp.Extensions.Web
{
	[DependsOn(
		typeof(AbpAspNetCoreMvcModule),
		typeof(AbpAspNetCoreMultiTenancyModule))]
	public class IsapAbpExtensionsWebModule: AbpModule
	{
		public override void PreConfigureServices(ServiceConfigurationContext context)
		{
			PreConfigure<IMvcBuilder>(mvcBuilder =>
				{
					mvcBuilder.AddNewtonsoftJson();
				});
		}
	}
}
