using System;
using System.Threading.Tasks;
using Isap.Abp.Extensions.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.DependencyInjection;

namespace MyCompanyName.MyProjectName.EntityFrameworkCore
{
    [ExposeServices(typeof(IAbpExtDbSchemaMigrator))]
    public class EntityFrameworkCoreAuditDbSchemaMigrator
        : IAbpExtDbSchemaMigrator, ITransientDependency
    {
        private readonly IServiceProvider _serviceProvider;

        public EntityFrameworkCoreAuditDbSchemaMigrator(
            IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task MigrateAsync()
        {
            await _serviceProvider
                .GetRequiredService<AuditMigrationsDbContext>()
                .Database
                .MigrateAsync();
        }
    }
}
