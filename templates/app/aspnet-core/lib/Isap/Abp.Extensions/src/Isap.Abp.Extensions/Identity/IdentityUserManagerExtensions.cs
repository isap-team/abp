using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Volo.Abp.Identity;
using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace Isap.Abp.Extensions.Identity
{
	public static class IdentityUserManagerExtensions
	{
		public static async Task EnsureRoleGrantedAsync(this IdentityUserManager userManager, IdentityUser user, string role)
		{
			if (await userManager.IsInRoleAsync(user, role)) return;
			(await userManager.AddToRoleAsync(user, role)).CheckErrors();
		}
	}
}
