using Isap.Abp.Extensions.Data;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace Isap.Abp.BackgroundJobs.EntityFrameworkCore.PostgreSql
{
	[DependsOn(
		typeof(IsapAbpBackgroundJobsEntityFrameworkCoreModule)
	)]
	public class IsapAbpBackgroundJobsPostgreSqlModule: BackgroundJobsDbModuleBase<BackgroundJobsPostgreSqlModelBuilder>
	{
		public const string ProviderKey = "PostgreSql";

		protected override string DbProviderKey => ProviderKey;

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
