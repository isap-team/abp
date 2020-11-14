using Isap.Abp.Extensions.Expressions.Predicates;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace Isap.Abp.Extensions.EntityFrameworkCore
{
	[DependsOn(
		typeof(AbpEntityFrameworkCoreModule),
		typeof(IsapAbpExtensionsModule)
	)]
	public class IsapAbpExtensionsEntityFrameworkCoreModule: AbpModule
	{
		public override void ConfigureServices(ServiceConfigurationContext context)
		{
			//context.Services.AddSingleton(NullDataFilterProvider.Instance);
			context.Services.AddSingleton(DefaultPredicateBuilder.Instance);
		}
	}
}
