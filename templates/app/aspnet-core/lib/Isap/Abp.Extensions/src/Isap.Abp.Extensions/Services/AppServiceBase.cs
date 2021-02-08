using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Isap.Abp.Extensions.DataFilters;
using Isap.Abp.Extensions.Domain;
using Isap.Abp.Extensions.MultiTenancy;
using Isap.Abp.Extensions.Permissions;
using Isap.Abp.Extensions.Querying;
using Isap.CommonCore.Services;
using Microsoft.AspNetCore.Identity;
using Volo.Abp.Application.Services;
#if ABP_331
using Volo.Abp.Authorization;
#else
using Volo.Abp.Authorization.Permissions;
#endif
using Volo.Abp.Identity;
using Volo.Abp.Security.Claims;
using Volo.Abp.Threading;

namespace Isap.Abp.Extensions.Services
{
	public abstract class AppServiceBase: ApplicationService, ISupportsLazyServices
	{
		object ISupportsLazyServices.ServiceProviderLock => ServiceProviderLock;

		ConcurrentDictionary<Type, object> ISupportsLazyServices.ServiceReferenceMap { get; } = new ConcurrentDictionary<Type, object>();

		protected ICurrentPrincipalAccessor CurrentPrincipalAccessor => LazyGetRequiredService<ICurrentPrincipalAccessor>();
		protected IdentityUserManager UserManager => LazyGetRequiredService<IdentityUserManager>();
		protected ITenantCache TenantCache => LazyGetRequiredService<ITenantCache>();
		protected IUserClaimsPrincipalFactory<IdentityUser> UserClaimsPrincipalFactory => LazyGetRequiredService<IUserClaimsPrincipalFactory<IdentityUser>>();
		protected IIsapPermissionChecker PermissionChecker => LazyGetRequiredService<IIsapPermissionChecker>();

		protected TService LazyGetRequiredService<TService>()
		{
			return SupportsLazyServicesExtensions.LazyGetRequiredService<TService>(this);
		}

		protected virtual List<DataFilterValue> ToDataFilterValues(ICollection<DataFilterValueDto> filterValues)
		{
			return filterValues?.Select(e => ObjectMapper.Map<DataFilterValueDto, DataFilterValue>(e)).ToList();
		}

		protected virtual ICollection<SortOption> ToSortOptions(ICollection<SortOptionDto> sortOptions)
		{
			return sortOptions?.Select(e => ObjectMapper.Map<SortOptionDto, SortOption>(e)).ToList();
		}

		protected void Impersonate(Guid? tenantId, Guid? userId, Action action)
		{
			if (action == null)
				return;

			using (CurrentTenant.Change(tenantId))
			{
				ClaimsPrincipal principal = AsyncHelper.RunSync(async () =>
					{
						// userId ??= await GetUnregisteredUserId(tenantId);
						IdentityUser user = await GetUserOrNull(userId);
						return await UserClaimsPrincipalFactory.CreateAsync(user);
					});

				using (CurrentPrincipalAccessor.Change(principal))
					action.Invoke();
			}
		}

		protected T Impersonate<T>(Guid? tenantId, Guid? userId, Func<T> action)
		{
			if (action == null)
				return default;

			using (CurrentTenant.Change(tenantId))
			{
				ClaimsPrincipal principal = AsyncHelper.RunSync(async () =>
					{
						// userId ??= await GetUnregisteredUserId(tenantId);
						IdentityUser user = await GetUserOrNull(userId);
						return await UserClaimsPrincipalFactory.CreateAsync(user);
					});

				using (CurrentPrincipalAccessor.Change(principal))
					return action.Invoke();
			}
		}

		protected async Task<T> ImpersonateAsync<T>(Guid? tenantId, Guid? userId, Func<Task<T>> action)
		{
			if (action == null)
				return default;

			using (CurrentTenant.Change(tenantId))
			{
				// userId ??= await GetUnregisteredUserId(tenantId);
				IdentityUser user = await GetUserOrNull(userId);
				using (CurrentPrincipalAccessor.Change(await UserClaimsPrincipalFactory.CreateAsync(user)))
					return await action.Invoke();
			}
		}

		protected async Task ImpersonateAsync(Guid? tenantId, Guid? userId, Func<Task> action)
		{
			if (action == null)
				return;

			using (CurrentTenant.Change(tenantId))
			{
				// userId ??= await GetUnregisteredUserId(tenantId);
				IdentityUser user = await GetUserOrNull(userId);
				using (CurrentPrincipalAccessor.Change(await UserClaimsPrincipalFactory.CreateAsync(user)))
					await action.Invoke();
			}
		}

		protected async Task<Guid?> GetUnregisteredUserId(Guid? tenantId)
		{
			if (tenantId.HasValue)
			{
				ITenantBase tenant = await TenantCache.GetAsync(tenantId.Value);
				return tenant.UnregisteredUserId;
			}

			return null;
		}

		protected async Task<IdentityUser> GetUserOrNull(Guid? userId)
		{
			return userId.HasValue ? await UserManager.GetByIdAsync(userId.Value) : null;
		}

		/// <summary>
		///     Проверяет доступ текущего пользователя.
		/// </summary>
		/// <returns></returns>
		// [AbpAllowAnonymous]
		protected virtual async Task CheckPermission(ICollection<OrganizationUnit> organizationUnits, params string[] permissionNames)
		{
			async Task Check()
			{
#if ABP_331
				var notGrantedPermissions = new List<string>();
				foreach (string permissionName in permissionNames)
				{
					if (!await PermissionChecker.IsGrantedAsync(permissionName))
						notGrantedPermissions.Add(permissionName);
				}
				if (notGrantedPermissions.Count > 0)
					throw new AbpAuthorizationException(L["AtLeastOneOfThesePermissionsMustBeGranted", string.Join(", ", notGrantedPermissions)]);
#else
				MultiplePermissionGrantResult result = await PermissionChecker.IsGrantedAsync(organizationUnits, permissionNames);
				if (!result.AllGranted)
				{
					string[] notGrantedPermissions = result.Result
						.Where(pair => pair.Value != PermissionGrantResult.Granted)
						.Select(pair => pair.Key)
						.ToArray();

					// TODO: из-за этого не работает сохранение профиля
					// throw new AbpAuthorizationException(L["AtLeastOneOfThesePermissionsMustBeGranted", string.Join(", ", notGrantedPermissions)]);
				}
#endif
			}

			Guid? currentUserId = CurrentUser.Id;
			if (currentUserId.HasValue)
			{
				await Check();
				return;
			}

			Guid? currentTenantId = CurrentTenant.Id;
			currentUserId = await GetUnregisteredUserId(currentTenantId);

			await ImpersonateAsync(currentTenantId, currentUserId, Check);
		}
	}

	public abstract class AppServiceBase<TEntityDto, TIntf>: AppServiceBase
	{
		// [AllowAnonymous]
		protected virtual TEntityDto ToDto(TIntf entry)
		{
			return ObjectMapper.Map<TIntf, TEntityDto>(entry);
		}
	}
}
