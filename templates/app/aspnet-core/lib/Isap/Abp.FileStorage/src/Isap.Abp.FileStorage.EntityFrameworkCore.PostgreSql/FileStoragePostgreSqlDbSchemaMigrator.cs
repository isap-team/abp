using System;
using System.Threading.Tasks;
using Isap.Abp.Extensions.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;

namespace Isap.Abp.FileStorage.EntityFrameworkCore.PostgreSql
{
	public class FileStoragePostgreSqlDbSchemaMigrator
		: IAbpExtDbSchemaMigrator, ITransientDependency
	{
		private readonly IServiceProvider _serviceProvider;

		public FileStoragePostgreSqlDbSchemaMigrator(
			IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}

		public ILogger<FileStoragePostgreSqlDbSchemaMigrator> Logger { get; set; }

		public async Task MigrateAsync()
		{
			try
			{
				var dbContext = _serviceProvider.GetRequiredService<FileStoragePostgreSqlMigrationsDbContext>();
				Logger.LogInformation(
					$"Migrating PostgreSQL fileStorage database using connection string: {dbContext.Database.GetDbConnection().ConnectionString}...");
				await dbContext.Database.MigrateAsync();
			}
			catch (Exception exception)
			{
				Logger.LogError(exception, "Error migrating PostgreSQL fileStorage database.");
				throw;
			}
		}
	}
}
