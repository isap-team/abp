using Volo.Abp.AspNetCore.MultiTenancy;
using Volo.Abp.Modularity;

namespace Isap.Abp.Extensions.Web
{
	[DependsOn(
		typeof(AbpAspNetCoreMultiTenancyModule))]
	public class IsapAbpExtensionsWebModule: AbpModule
	{
	}
}
