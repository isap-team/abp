using System;
using System.Threading.Tasks;
using Isap.Abp.Extensions.Caching;
using Isap.Abp.Extensions.Settings;
using Isap.Converters;
using Volo.Abp.AspNetCore.Mvc.MultiTenancy;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
using Volo.Abp.SettingManagement;
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
		public IValueConverter Converter { get; set; }
		public ISettingManager SettingManager { get; set; }

		protected override async Task<TenantCacheItem> TryLoadItem(Guid id)
		{
			FindTenantResultDto tenant = await TenantAppService.FindTenantByIdAsync(id);
			return await TryLoadItem(tenant);
		}

		protected virtual async Task<TenantCacheItem> TryLoadItem(FindTenantResultDto tenant)
		{
			if (!tenant.Success || !tenant.TenantId.HasValue)
				return null;
			string unregisteredUserId = await SettingManager.GetOrNullForTenantAsync(IsapAbpExtensionsSettings.UnregisteredUserId, tenant.TenantId.Value);
			return new TenantCacheItem(tenant.TenantId.Value, tenant.Name, Converter.TryConvertTo<Guid>(unregisteredUserId).AsDefaultIfNotSuccess());
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
					return await TryLoadItem(tenant);
				});
		}
	}
}
