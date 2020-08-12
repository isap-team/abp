using Microsoft.EntityFrameworkCore;
using Volo.Abp.AuditLogging;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace MyCompanyName.MyProjectName.EntityFrameworkCore
{
    [ConnectionStringName(AbpAuditLoggingDbProperties.ConnectionStringName)]
    public class AuditMigrationsDbContext : AbpDbContext<AuditMigrationsDbContext>
    {
        public AuditMigrationsDbContext(DbContextOptions<AuditMigrationsDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ConfigureAuditLogging();
        }
    }
}
