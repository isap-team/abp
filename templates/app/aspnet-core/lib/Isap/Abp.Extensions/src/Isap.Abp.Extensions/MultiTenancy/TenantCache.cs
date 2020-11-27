using System;
using System.Threading.Tasks;
using Isap.Abp.Extensions.Caching;
using Volo.Abp.AspNetCore.Mvc.MultiTenancy;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Threading;

namespace Isap.Abp.Extensions.MultiTenancy
{
	public interface ITenantCache: IDistributedEntityCache<ITenantBase, Guid>
	{
		ITenantBase Get(string name);
		Task<ITenantBase> GetAsync(string name);
		ITenantBase GetOrNull(string name);
		Task<ITenantBase> GetOrNullAsync(string name);
	}

	public class TenantCache: DistributedEntityCacheBase<ITenantBase, TenantCacheItem, Guid>, ITenantCache, ISingletonDependency
	{
		protected override string EntityName => "Tenant";

		public IAbpTenantAppService TenantAppService { get; set; }
		public IDistributedCache<CacheRef<TenantCacheItem, Guid>, string> ByNameIndex { get; set; }

		protected override async Task<TenantCacheItem> TryLoadItem(Guid id)
		{
			FindTenantResultDto tenant = await TenantAppService.FindTenantByIdAsync(id);
			return tenant.Success && tenant.TenantId.HasValue ? new TenantCacheItem(tenant.TenantId.Value, tenant.Name) : null;
		}

		public ITenantBase Get(string name)
		{
			return GetOrNull(name) ?? throw new InvalidOperationException($"Can't find tenant with name '{name}'.");
		}

		public async Task<ITenantBase> GetAsync(string name)
		{
			return await GetOrNullAsync(name) ?? throw new InvalidOperationException($"Can't find tenant with name '{name}'.");
		}

		public ITenantBase GetOrNull(string name)
		{
			return AsyncHelper.RunSync(() => GetOrNullAsync(name));
		}

		public async Task<ITenantBase> GetOrNullAsync(string name)
		{
			return await InternalGetOrNullAsync(ByNameIndex, name, async n =>
				{
					FindTenantResultDto tenant = await TenantAppService.FindTenantByNameAsync(n);
					return tenant.Success && tenant.TenantId.HasValue ? new TenantCacheItem(tenant.TenantId.Value, tenant.Name) : null;
				});
		}
	}
}
