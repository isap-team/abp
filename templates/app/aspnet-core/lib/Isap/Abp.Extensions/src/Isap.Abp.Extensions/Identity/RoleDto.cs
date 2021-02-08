using System;
using System.Collections.Generic;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.PermissionManagement;

namespace Isap.Abp.Extensions.Identity
{
	public class RoleDto: IdentityRoleDto, IMultiTenant
	{
		public Guid? TenantId { get; set; }

		public List<PermissionGrantInfoDto> Permissions { get; set; }
	}
}
