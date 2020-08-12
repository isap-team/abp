using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace Isap.Hosting
{
	[DependsOn(
		typeof(AbpAutofacModule)
	)]
	public class IsapAbpHostingModule: AbpModule
	{
	}
}
