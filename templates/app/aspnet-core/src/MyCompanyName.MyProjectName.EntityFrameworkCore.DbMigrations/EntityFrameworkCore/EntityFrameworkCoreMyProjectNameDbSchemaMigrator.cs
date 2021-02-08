using System;
using System.Threading.Tasks;
using Isap.Abp.Extensions.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.DependencyInjection;

namespace MyCompanyName.MyProjectName.EntityFrameworkCore
{
    [ExposeServices(typeof(IAbpExtDbSchemaMigrator))]
    public class EntityFrameworkCoreMyProjectNameDbSchemaMigrator
        : IAbpExtDbSchemaMigrator, ITransientDependency
    {
        private readonly IServiceProvider _serviceProvider;

        public EntityFrameworkCoreMyProjectNameDbSchemaMigrator(
            IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task MigrateAsync()
        {
            /* We intentionally resolving the MyProjectNameMigrationsDbContext
             * from IServiceProvider (instead of directly injecting it)
             * to properly get the connection string of the current tenant in the
             * current scope.
             */

            await _serviceProvider
                .GetRequiredService<MyProjectNameMigrationsDbContext>()
                .Database
                .MigrateAsync();
        }
    }
}
