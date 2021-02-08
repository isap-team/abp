using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Volo.Abp.AuditLogging;

namespace MyCompanyName.MyProjectName.EntityFrameworkCore
{
    public class AuditMigrationsDbContextFactory: IDesignTimeDbContextFactory<AuditMigrationsDbContext>
    {
        public AuditMigrationsDbContext CreateDbContext(string[] args)
        {
            MyProjectNameEfCoreEntityExtensionMappings.Configure();

            var configuration = BuildConfiguration();

            var builder = new DbContextOptionsBuilder<AuditMigrationsDbContext>()
                .UseNpgsql(configuration.GetConnectionString(AbpAuditLoggingDbProperties.ConnectionStringName));

            return new AuditMigrationsDbContext(builder.Options);
        }

        private static IConfigurationRoot BuildConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false);

            return builder.Build();
        }
    }
}
