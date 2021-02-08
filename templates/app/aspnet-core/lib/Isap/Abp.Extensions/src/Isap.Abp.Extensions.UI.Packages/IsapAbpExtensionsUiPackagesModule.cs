using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.Modularity;

namespace Isap.Abp.Extensions.UI.Packages
{
	[DependsOn(typeof(AbpAspNetCoreMvcUiBundlingModule))]
	public class IsapAbpExtensionsUiPackagesModule: AbpModule
	{
	}
}
