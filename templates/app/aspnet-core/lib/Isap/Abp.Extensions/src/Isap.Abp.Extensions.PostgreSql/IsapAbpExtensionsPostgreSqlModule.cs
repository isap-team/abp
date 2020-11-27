using Isap.Abp.Extensions.Expressions.Predicates;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Volo.Abp.EntityFrameworkCore.PostgreSql;
using Volo.Abp.Modularity;

namespace Isap.Abp.Extensions.PostgreSql
{
	[DependsOn(
		typeof(AbpEntityFrameworkCorePostgreSqlModule),
		typeof(IsapAbpExtensionsModule)
	)]
	public class IsapAbpExtensionsPostgreSqlModule: AbpModule
	{
		public override void ConfigureServices(ServiceConfigurationContext context)
		{
			base.ConfigureServices(context);

			context.Services.Replace(ServiceDescriptor.Singleton<IPredicateBuilder, NpgsqlPredicateBuilder>());
		}
	}
}
