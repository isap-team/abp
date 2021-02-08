using System;
using System.Threading.Tasks;
using Isap.Abp.Extensions.Caching;
using Isap.CommonCore.Services;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Threading;

namespace Isap.Abp.Extensions.Identity
{
	public class RoleCacheItem: CommonEntityDto<Guid>, IRoleBase
	{
		public Guid? TenantId { get; set; }

		public string Name { get; set; }

		public bool IsDefault { get; set; }
		public bool IsStatic { get; set; }
		public bool IsPublic { get; set; }
	}

	public interface IRoleCache: IDistributedEntityCache<IRoleBase, Guid>
	{
		IRoleBase Get(Guid? tenantId, string name);
		Task<IRoleBase> GetAsync(Guid? tenantId, string name);
		IRoleBase GetOrNull(Guid? tenantId, string name);
		Task<IRoleBase> GetOrNullAsync(Guid? tenantId, string name);

		IRoleBase Get(string name);
		Task<IRoleBase> GetAsync(string name);
		IRoleBase GetOrNull(string name);
		Task<IRoleBase> GetOrNullAsync(string name);
	}

	public class RoleCache: DistributedEntityCacheBase<IRoleBase, RoleCacheItem, Guid>, IRoleCache, ISingletonDependency
	{
		protected override string EntityName => "UserRole";

		protected IDistributedCache<CacheRef<RoleCacheItem, Guid>, string> ByNameIndex =>
			LazyGetRequiredService<IDistributedCache<CacheRef<RoleCacheItem, Guid>, string>>();

		protected ICurrentTenant CurrentTenant => LazyGetRequiredService<ICurrentTenant>();
		protected IdentityRoleManager RoleManager => LazyGetRequiredService<IdentityRoleManager>();

		public IRoleBase Get(Guid? tenantId, string name)
		{
			return GetOrNull(tenantId, name)
				?? throw new InvalidOperationException($"Can't find role with name '{name}' and tenant id '{tenantId}'.")
				;
		}

		public async Task<IRoleBase> GetAsync(Guid? tenantId, string name)
		{
			return await GetOrNullAsync(tenantId, name)
				?? throw new InvalidOperationException($"Can't find role with name '{name}' and tenant id '{tenantId}'.")
				;
		}

		public IRoleBase GetOrNull(Guid? tenantId, string name)
		{
			return AsyncHelper.RunSync(() => GetOrNullAsync(tenantId, name));
		}

		public async Task<IRoleBase> GetOrNullAsync(Guid? tenantId, string name)
		{
			return await InternalGetOrNullAsync(ByNameIndex, GetKey(tenantId, name), async key =>
				{
					using (CurrentTenant.Change(tenantId))
					{
						IdentityRole role = await RoleManager.FindByNameAsync(name);
						if (role == null)
							throw new InvalidOperationException($"Can't find role with name '{name}' and tenant id '{tenantId}'.");
						return await TryLoadItem(role);
					}
				});
		}

		public IRoleBase Get(string name) => Get(CurrentTenant.Id, name);

		public Task<IRoleBase> GetAsync(string name) => GetAsync(CurrentTenant.Id, name);

		public IRoleBase GetOrNull(string name) => GetOrNull(CurrentTenant.Id, name);

		public Task<IRoleBase> GetOrNullAsync(string name) => GetOrNullAsync(CurrentTenant.Id, name);

		protected override async Task<RoleCacheItem> TryLoadItem(Guid id)
		{
			IdentityRole role = await RoleManager.GetByIdAsync(id);
			return await TryLoadItem(role);
		}

		protected Task<RoleCacheItem> TryLoadItem(IdentityRole role)
		{
			return Task.FromResult(new RoleCacheItem
				{
					Id = role.Id,
					TenantId = role.TenantId,
					Name = role.Name,
					IsDefault = role.IsDefault,
					IsStatic = role.IsStatic,
					IsPublic = role.IsPublic,
				});
		}

		protected string GetKey(Guid? tenantId, string name) => $"{tenantId ?? Guid.Empty:N}:{name}";
	}
}
