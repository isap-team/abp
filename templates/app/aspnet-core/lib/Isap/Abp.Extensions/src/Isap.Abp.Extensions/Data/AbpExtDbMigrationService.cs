using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Isap.CommonCore.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;
using Volo.Abp.TenantManagement;

namespace Isap.Abp.Extensions.Data
{
	public interface IAbpExtDbMigrationService
	{
		Task MigrateAsync(object args = null);
	}

	public static class IsapDataSeederExtensions
	{
		public const string AppDataDirPropertyName = "#AppDataDir";

		public static Task SeedAsync(this IDataSeeder seeder, Guid? tenantId, DirectoryInfo appDataDir, object args)
		{
			var context = new DataSeedContext(tenantId);
			context.Properties[AppDataDirPropertyName] = appDataDir;
			context.Properties.Assign(args.AsNameToObjectMap());
			return seeder.SeedAsync(context);
		}
	}

	public class AbpExtDbMigrationService: IAbpExtDbMigrationService, ITransientDependency
	{
		private readonly IDataSeeder _dataSeeder;
		private readonly IEnumerable<IAbpExtDbSchemaMigrator> _dbSchemaMigrators;
		private readonly ITenantRepository _tenantRepository;
		private readonly ICurrentTenant _currentTenant;

		public AbpExtDbMigrationService(
			IDataSeeder dataSeeder,
			IEnumerable<IAbpExtDbSchemaMigrator> dbSchemaMigrators,
			ITenantRepository tenantRepository,
			ICurrentTenant currentTenant)
		{
			_dataSeeder = dataSeeder;
			_dbSchemaMigrators = dbSchemaMigrators;
			_tenantRepository = tenantRepository;
			_currentTenant = currentTenant;

			Logger = NullLogger<AbpExtDbMigrationService>.Instance;
		}

		public ILogger<AbpExtDbMigrationService> Logger { get; set; }

		public async Task MigrateAsync(object args = null)
		{
			Logger.LogInformation("Started database migrations...");

			await MigrateDatabaseSchemaAsync();
			await SeedDataAsync(null, args);

			Logger.LogInformation($"Successfully completed host database migrations.");

			var tenants = await _tenantRepository.GetListAsync(includeDetails: true);

			var migratedDatabaseSchemas = new HashSet<string>();
			foreach (var tenant in tenants)
			{
				using (_currentTenant.Change(tenant.Id))
				{
					if (tenant.ConnectionStrings.Any())
					{
						var tenantConnectionStrings = tenant.ConnectionStrings
							.Select(x => x.Value)
							.ToList();

						if (!migratedDatabaseSchemas.IsSupersetOf(tenantConnectionStrings))
						{
							await MigrateDatabaseSchemaAsync(tenant);

							migratedDatabaseSchemas.AddIfNotContains(tenantConnectionStrings);
						}
					}

					await SeedDataAsync(tenant, args);
				}

				Logger.LogInformation($"Successfully completed {tenant.Name} tenant database migrations.");
			}

			Logger.LogInformation("Successfully completed database migrations.");
		}

		private async Task MigrateDatabaseSchemaAsync(Tenant tenant = null)
		{
			Logger.LogInformation(
				$"Migrating schema for {(tenant == null ? "host" : tenant.Name + " tenant")} database...");

			foreach (var migrator in _dbSchemaMigrators)
			{
				await migrator.MigrateAsync();
			}
		}

		private async Task SeedDataAsync(Tenant tenant, object args)
		{
			Logger.LogInformation($"Executing {(tenant == null ? "host" : tenant.Name + " tenant")} database seed...");

			var appDataDir = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, "App_Data"));
			if (appDataDir.Exists)
			{
				if (tenant != null)
				{
					DirectoryInfo tenantsDir = appDataDir.GetDirectories("Tenants").FirstOrDefault();
					if (tenantsDir != null)
					{
						DirectoryInfo tenantAppDataDir = tenantsDir.GetDirectories(tenant.Name).FirstOrDefault();
						if (tenantAppDataDir != null)
							appDataDir = tenantAppDataDir;
					}
				}
			}

			await _dataSeeder.SeedAsync(tenant?.Id, appDataDir, args);
		}
	}
}
