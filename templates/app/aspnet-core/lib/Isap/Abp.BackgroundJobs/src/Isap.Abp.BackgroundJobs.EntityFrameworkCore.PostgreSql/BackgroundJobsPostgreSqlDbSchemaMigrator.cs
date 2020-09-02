using System;
using System.Threading.Tasks;
using Isap.Abp.Extensions.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;

namespace Isap.Abp.BackgroundJobs.EntityFrameworkCore.PostgreSql
{
	public class BackgroundJobsPostgreSqlDbSchemaMigrator: IAbpExtDbSchemaMigrator, ITransientDependency
	{
		private readonly IServiceProvider _serviceProvider;

		public BackgroundJobsPostgreSqlDbSchemaMigrator(
			IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}

		public ILogger<BackgroundJobsPostgreSqlDbSchemaMigrator> Logger { get; set; }

		public async Task MigrateAsync()
		{
			try
			{
				var dbContext = _serviceProvider.GetRequiredService<BackgroundJobsPostgreSqlMigrationsDbContext>();
				Logger.LogInformation(
					$"Migrating PostgreSQL background jobs database using connection string: {dbContext.Database.GetDbConnection().ConnectionString}...");
				await dbContext.Database.MigrateAsync();
			}
			catch (Exception exception)
			{
				Logger.LogError(exception, "Error migrating PostgreSQL background jobs database.");
				throw;
			}
		}
	}
}
