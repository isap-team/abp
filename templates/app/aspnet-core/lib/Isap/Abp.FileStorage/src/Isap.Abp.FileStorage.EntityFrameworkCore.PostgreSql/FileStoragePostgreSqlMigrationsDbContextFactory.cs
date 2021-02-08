using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Isap.Abp.FileStorage.EntityFrameworkCore.PostgreSql
{
	public class FileStoragePostgreSqlMigrationsDbContextFactory
		: IDesignTimeDbContextFactory<FileStoragePostgreSqlMigrationsDbContext>
	{
		public FileStoragePostgreSqlMigrationsDbContext CreateDbContext(string[] args)
		{
			var configuration = BuildConfiguration();

			var builder = new DbContextOptionsBuilder<FileStoragePostgreSqlMigrationsDbContext>()
				.UseNpgsql(configuration.GetConnectionString(FileStorageDbProperties.ConnectionStringName));

			return new FileStoragePostgreSqlMigrationsDbContext(builder.Options);
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
