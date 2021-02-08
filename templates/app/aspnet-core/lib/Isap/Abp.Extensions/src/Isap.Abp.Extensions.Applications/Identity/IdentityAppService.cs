using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Isap.Abp.Extensions.Services;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.PermissionManagement;

namespace Isap.Abp.Extensions.Identity
{
	public class IdentityAppService: AppServiceBase, IIdentityAppService
	{
		protected IdentityRoleManager RoleManager => LazyGetRequiredService<IdentityRoleManager>();
		protected IPermissionManager PermissionManager => LazyGetRequiredService<IPermissionManager>();
		protected IPermissionDefinitionManager PermissionDefinitionManager => LazyGetRequiredService<IPermissionDefinitionManager>();

		public async Task<RoleDto> RoleExport(Guid roleId)
		{
			IdentityRole role = await RoleManager.GetByIdAsync(roleId);
			RoleDto result = ObjectMapper.Map<IdentityRole, RoleDto>(role);
			result.Permissions = new List<PermissionGrantInfoDto>();

			var multiTenancySide = CurrentTenant.GetMultiTenancySide();

			foreach (var group in PermissionDefinitionManager.GetGroups())
			{
				foreach (var permission in group.GetPermissionsWithChildren())
				{
					if (!permission.IsEnabled)
					{
						continue;
					}

					if (permission.Providers.Any() && !permission.Providers.Contains(RolePermissionValueProvider.ProviderName))
					{
						continue;
					}

					if (!permission.MultiTenancySide.HasFlag(multiTenancySide))
					{
						continue;
					}

					var grantInfoDto = new PermissionGrantInfoDto
						{
							Name = permission.Name,
							DisplayName = permission.DisplayName.Localize(StringLocalizerFactory),
							ParentName = permission.Parent?.Name,
							AllowedProviders = permission.Providers,
							GrantedProviders = new List<ProviderInfoDto>(),
						};

					var grantInfo = await PermissionManager.GetAsync(permission.Name, RolePermissionValueProvider.ProviderName, role.Name);

					grantInfoDto.IsGranted = grantInfo.IsGranted;

					foreach (var provider in grantInfo.Providers)
					{
						grantInfoDto.GrantedProviders.Add(new ProviderInfoDto
							{
								ProviderName = provider.Name,
								ProviderKey = provider.Key,
							});
					}

					result.Permissions.Add(grantInfoDto);
				}
			}

			return result;
		}
	}
}
