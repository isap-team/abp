using Isap.Abp.Extensions.Data;
using Isap.Abp.Extensions.PostgreSql;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.PostgreSql;
using Volo.Abp.Modularity;

namespace Isap.Abp.BackgroundJobs.EntityFrameworkCore.PostgreSql
{
	[DependsOn(
		typeof(AbpEntityFrameworkCorePostgreSqlModule),
		typeof(IsapAbpBackgroundJobsEntityFrameworkCoreModule),
		typeof(IsapAbpExtensionsPostgreSqlModule))]
	public class IsapAbpBackgroundJobsPostgreSqlModule: BackgroundJobsDbModuleBase<BackgroundJobsPostgreSqlModelBuilder>
	{
		protected override string DbProviderKey => IsapAbpPostgreSqlConsts.DbProviderKey;

		protected override void ConfigureDbContextOptions(ServiceConfigurationContext context, AbpDbContextOptions options)
		{
			options.UseNpgsql();
		}

		protected override void ConfigureDatabaseSpecificServices(ServiceConfigurationContext context)
		{
			base.ConfigureDatabaseSpecificServices(context);

			context.Services.AddTransient<IAbpExtDbSchemaMigrator, BackgroundJobsPostgreSqlDbSchemaMigrator>();
		}

		protected override void ConfigureMainDbContext(ServiceConfigurationContext context)
		{
			context.Services.AddAbpDbContext<BackgroundJobsDbContext>(options => options.AddDefaultRepositories());
		}

		protected override void ConfigureMigrationsDbContext(ServiceConfigurationContext context)
		{
			context.Services.AddAbpDbContext<BackgroundJobsPostgreSqlMigrationsDbContext>(options => options.AddDefaultRepositories());
		}
	}
}
