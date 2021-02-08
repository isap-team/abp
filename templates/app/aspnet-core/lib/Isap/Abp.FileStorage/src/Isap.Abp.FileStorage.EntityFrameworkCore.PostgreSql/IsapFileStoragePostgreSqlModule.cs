using Isap.Abp.Extensions.Data;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.PostgreSql;
using Volo.Abp.Modularity;

namespace Isap.Abp.FileStorage.EntityFrameworkCore.PostgreSql
{
	[DependsOn(
		typeof(AbpEntityFrameworkCorePostgreSqlModule),
		typeof(FileStorageEntityFrameworkCoreModule)
	)]
	public class IsapFileStoragePostgreSqlModule
		: FileStorageDbModuleBase<FileStoragePostgreSqlModelBuilder>
	{
		public const string ProviderKey = "PostgreSql";
		protected override string DbProviderKey => ProviderKey;

		protected override void ConfigureDbContextOptions(ServiceConfigurationContext context, AbpDbContextOptions options)
		{
			options.UseNpgsql();
		}

		protected override void ConfigureMainDbContext(ServiceConfigurationContext context)
		{
			context.Services.AddAbpDbContext<FileStorageDbContext>(options => options.AddDefaultRepositories());
		}

		protected override void ConfigureMigrationsDbContext(ServiceConfigurationContext context)
		{
			context.Services.AddAbpDbContext<FileStoragePostgreSqlMigrationsDbContext>(options => options.AddDefaultRepositories());
		}

		protected override void ConfigureDatabaseSpecificServices(ServiceConfigurationContext context)
		{
			base.ConfigureDatabaseSpecificServices(context);

			context.Services.AddTransient<IAbpExtDbSchemaMigrator, FileStoragePostgreSqlDbSchemaMigrator>();
		}
	}
}
