using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Isap.Abp.BackgroundJobs.EntityFrameworkCore.PostgreSql
{
	public class BackgroundJobsPostgreSqlMigrationsDbContextFactory: IDesignTimeDbContextFactory<BackgroundJobsPostgreSqlMigrationsDbContext>
	{
		public BackgroundJobsPostgreSqlMigrationsDbContext CreateDbContext(string[] args)
		{
			var configuration = BuildConfiguration();

			var builder = new DbContextOptionsBuilder<BackgroundJobsPostgreSqlMigrationsDbContext>()
				.UseNpgsql(configuration.GetConnectionString(BackgroundJobsDbProperties.ConnectionStringName));

			return new BackgroundJobsPostgreSqlMigrationsDbContext(builder.Options);
		}

		private static IConfigurationRoot BuildConfiguration()
		{
			var builder = new ConfigurationBuilder()
					.SetBasePath(Directory.GetCurrentDirectory())
					.AddJsonFile("appsettings.json", optional: false)
					.AddJsonFile("appsettings.Development.json", optional: true)
				;

			return builder.Build();
		}
	}
}
