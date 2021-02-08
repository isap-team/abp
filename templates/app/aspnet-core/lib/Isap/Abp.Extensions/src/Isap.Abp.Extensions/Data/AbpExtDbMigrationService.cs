using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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
			try
			{
				if (DbMigrationsProjectExists() && !MigrationsFolderExists())
				{
					AddInitialMigration();
					return;
				}
			}
			catch (Exception e)
			{
				Logger.LogWarning("Couldn't determinate if any migrations exist : " + e.Message);
			}

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

			Logger.LogInformation("Successfully completed all database migrations.");
			Logger.LogInformation("You can safely end this process...");
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

		private bool DbMigrationsProjectExists()
		{
			var dbMigrationsProjectFolder = GetDbMigrationsProjectFolderPath();

			return dbMigrationsProjectFolder != null;
		}

		private bool MigrationsFolderExists()
		{
			var dbMigrationsProjectFolder = GetDbMigrationsProjectFolderPath();

			return Directory.Exists(Path.Combine(dbMigrationsProjectFolder, "migrations"));
		}

		private void AddInitialMigration()
		{
			Logger.LogInformation("Creating initial migration...");

			string argumentPrefix;
			string fileName;

			if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) || RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
			{
				argumentPrefix = "-c";
				fileName = "/bin/bash";
			}
			else
			{
				argumentPrefix = "/C";
				fileName = "cmd.exe";
			}

			var procStartInfo = new ProcessStartInfo(fileName,
				$"{argumentPrefix} \"abp create-migration-and-run-migrator \"{GetDbMigrationsProjectFolderPath()}\"\""
			);

			try
			{
				Process.Start(procStartInfo);
			}
			catch (Exception)
			{
				throw new Exception("Couldn't run ABP CLI...");
			}
		}

		private string GetDbMigrationsProjectFolderPath()
		{
			var slnDirectoryPath = GetSolutionDirectoryPath();

			if (slnDirectoryPath == null)
			{
				throw new Exception("Solution folder not found!");
			}

			var srcDirectoryPath = Path.Combine(slnDirectoryPath, "src");

			return Directory.GetDirectories(srcDirectoryPath)
				.FirstOrDefault(d => d.EndsWith(".DbMigrations"));
		}

		private string GetSolutionDirectoryPath()
		{
			var currentDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());

			while (Directory.GetParent(currentDirectory.FullName) != null)
			{
				currentDirectory = Directory.GetParent(currentDirectory.FullName);

				if (Directory.GetFiles(currentDirectory.FullName).FirstOrDefault(f => f.EndsWith(".sln")) != null)
				{
					return currentDirectory.FullName;
				}
			}

			return null;
		}
	}
}
