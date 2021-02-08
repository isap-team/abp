using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;
using Volo.Abp.MultiTenancy.ConfigurationStore;
using Volo.Abp.TenantManagement;

namespace MyCompanyName.MyProjectName.MultiTenancy
{
    public class TenantDataSeedContributor: IDataSeedContributor, ITransientDependency
    {
        public TenantDataSeedContributor(IOptions<AbpDefaultTenantStoreOptions> defaultTenantStoreOptions)
        {
            DefaultTenantStoreOptions = defaultTenantStoreOptions.Value;
        }

        public AbpDefaultTenantStoreOptions DefaultTenantStoreOptions { get; }

        public ITenantRepository TenantRepository { get; set; }
        public ITenantManager TenantManager { get; set; }

        public async Task SeedAsync(DataSeedContext context)
        {
            if (context.TenantId.HasValue) return;

            foreach (TenantConfiguration tenantConfiguration in DefaultTenantStoreOptions.Tenants)
            {
                Tenant tenant = await TenantRepository.FindByNameAsync(tenantConfiguration.Name);
                if (tenant == null)
                {
                    tenant = await TenantManager.CreateAsync(tenantConfiguration.Name);
                    EntityHelper.TrySetId(tenant, () => tenantConfiguration.Id);
                    await TenantRepository.InsertAsync(tenant, autoSave: true);
                }
            }
        }
    }
}
