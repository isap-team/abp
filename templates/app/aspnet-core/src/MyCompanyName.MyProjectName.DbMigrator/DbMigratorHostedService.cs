using System;
using System.Threading;
using System.Threading.Tasks;
using Isap.Abp.Extensions.Data;
using Isap.Abp.Extensions.PostgreSql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyCompanyName.MyProjectName.MultiTenancy;
using Serilog;
using Volo.Abp;
using Volo.Abp.MultiTenancy.ConfigurationStore;

namespace MyCompanyName.MyProjectName.DbMigrator
{
    public class DbMigratorHostedService : IHostedService
    {
        private readonly IConfiguration _config;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;

        public DbMigratorHostedService(
            IConfiguration config,
            IHostApplicationLifetime hostApplicationLifetime)
        {
            _config = config;
            _hostApplicationLifetime = hostApplicationLifetime;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using (var application = AbpApplicationFactory.Create<MyProjectNameDbMigratorModule>(options =>
            {
                options.Services.Configure<AbpExtDbOptions>(dbOptions =>
                    {
                        dbOptions.IsMigrationMode = true;
                        dbOptions.DataProviderKey = _config[nameof(dbOptions.DataProviderKey)] ?? IsapAbpPostgreSqlConsts.DbProviderKey;
                    });
                options.Configuration.EnvironmentName = _config[nameof(options.Configuration.EnvironmentName)] ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                options.Services.Configure<AbpDefaultTenantStoreOptions>(storeOptions =>
                    {
                        storeOptions.Tenants = MultiTenancyConsts.DefaultTenants;
                    });
                options.UseAutofac();
                options.Services.AddLogging(c => c.AddSerilog());
            }))
            {
                application.Initialize();

                await application
                    .ServiceProvider
                    .GetRequiredService<IAbpExtDbMigrationService>()
                    .MigrateAsync();

                application.Shutdown();

                _hostApplicationLifetime.StopApplication();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
