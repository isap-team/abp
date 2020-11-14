using Isap.Abp.Extensions;
using Volo.Abp.Modularity;

namespace Isap.Abp.BackgroundJobs
{
	[DependsOn(
		typeof(IsapAbpExtensionsModule)
	)]
	public class IsapAbpBackgroundJobsDomainSharedModule: AbpModule
	{
	}
}
