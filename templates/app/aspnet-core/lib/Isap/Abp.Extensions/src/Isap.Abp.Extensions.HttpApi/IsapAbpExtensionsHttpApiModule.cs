using Volo.Abp.Modularity;

namespace Isap.Abp.Extensions
{
	[DependsOn(typeof(IsapAbpExtensionsModule))]
	public class IsapAbpExtensionsHttpApiModule: AbpModule
	{
	}
}
