using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Isap.Abp.Extensions.Domain;
using Isap.Abp.Extensions.Identity;
using JetBrains.Annotations;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Identity;
using Volo.Abp.Security.Claims;

namespace Isap.Abp.Extensions.Permissions
{
	public interface IIsapPermissionChecker
	{
		Task<bool> IsGrantedAsync([CanBeNull] ICollection<OrganizationUnit> organizationUnits, [NotNull]string name);

		Task<bool> IsGrantedAsync([CanBeNull] ClaimsPrincipal claimsPrincipal, [CanBeNull] ICollection<OrganizationUnit> organizationUnits, [NotNull]string name);

		Task<MultiplePermissionGrantResult> IsGrantedAsync([CanBeNull] ICollection<OrganizationUnit> organizationUnits, [NotNull]string[] names);

		Task<MultiplePermissionGrantResult> IsGrantedAsync([CanBeNull] ClaimsPrincipal claimsPrincipal, [CanBeNull] ICollection<OrganizationUnit> organizationUnits, [NotNull]string[] names);
	}

	public class IsapPermissionChecker: DomainServiceBase, IIsapPermissionChecker
	{
		protected IPermissionChecker PermissionChecker => LazyServiceProvider.LazyGetRequiredService<IPermissionChecker>();
		protected ICurrentPrincipalAccessor PrincipalAccessor => LazyServiceProvider.LazyGetRequiredService<ICurrentPrincipalAccessor>();
		protected IRoleCache RoleCache => LazyServiceProvider.LazyGetRequiredService<IRoleCache>();

		public async Task<bool> IsGrantedAsync(ICollection<OrganizationUnit> organizationUnits, string name)
		{
			return await IsGrantedAsync(PrincipalAccessor.Principal, organizationUnits, name);
		}

		public async Task<bool> IsGrantedAsync(ClaimsPrincipal claimsPrincipal, ICollection<OrganizationUnit> organizationUnits, string name)
		{
			return await PermissionChecker.IsGrantedAsync(PrepareClaimsPrincipal(claimsPrincipal, organizationUnits), name);
		}

		public async Task<MultiplePermissionGrantResult> IsGrantedAsync(ICollection<OrganizationUnit> organizationUnits, string[] names)
		{
			return await IsGrantedAsync(PrincipalAccessor.Principal, organizationUnits, names);
		}

		public async Task<MultiplePermissionGrantResult> IsGrantedAsync(ClaimsPrincipal claimsPrincipal, ICollection<OrganizationUnit> organizationUnits, string[] names)
		{
			return await PermissionChecker.IsGrantedAsync(PrepareClaimsPrincipal(claimsPrincipal, organizationUnits), names);
		}

		private ClaimsPrincipal PrepareClaimsPrincipal(ClaimsPrincipal claimsPrincipal, ICollection<OrganizationUnit> organizationUnits)
		{
			// TODO
			//if (organizationUnits != null)
			//{
			//	List<string> roles = organizationUnits
			//		.SelectMany(ou => ou.Roles.Select(r => r.RoleId))
			//		.Distinct()
			//		.Select(roleId => RoleCache.Get(roleId).Name)
			//		.ToList();

			//	if (roles.Count > 0)
			//	{
			//		List<Claim> claims = (claimsPrincipal?.Claims ?? Enumerable.Empty<Claim>()).ToList();
			//		roles.RemoveAll(claims.Where(claim => claim.Type == AbpClaimTypes.Role).Select(claim => claim.Value));

			//		var identity = new ClaimsIdentity(claims.Concat(roles.Select(r => new Claim(AbpClaimTypes.Role, r))));
			//		var principal = new ClaimsPrincipal(identity);
			//		return principal;
			//	}
			//}

			return claimsPrincipal;
		}
	}
}
