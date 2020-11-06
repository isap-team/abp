using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Volo.Abp.Data;

namespace Isap.Abp.BackgroundJobs.EntityFrameworkCore.PostgreSql
{
	public class BackgroundJobsPostgreSqlMigrationsDbContextFactory: IDesignTimeDbContextFactory<BackgroundJobsPostgreSqlMigrationsDbContext>
	{
		public BackgroundJobsPostgreSqlMigrationsDbContext CreateDbContext(string[] args)
		{
			var configuration = BuildConfiguration();

			string connectionString = configuration.GetConnectionString(BackgroundJobsDbProperties.ConnectionStringName)
					?? configuration.GetConnectionString(ConnectionStrings.DefaultConnectionStringName)
				;
			var builder = new DbContextOptionsBuilder<BackgroundJobsPostgreSqlMigrationsDbContext>()
				.UseNpgsql(connectionString);

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
