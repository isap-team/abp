using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;

namespace Isap.Abp.Extensions.Identity
{
	[RemoteService]
	[Route("api/isap/identity")]
	public class IdentityController: IsapControllerBase, IIdentityAppService
	{
		protected IIdentityAppService AppService => LazyGetRequiredService<IIdentityAppService>();

		[HttpGet]
		[Route("role-export")]
		public Task<RoleDto> RoleExport(Guid roleId)
		{
			return AppService.RoleExport(roleId);
		}
	}
}
