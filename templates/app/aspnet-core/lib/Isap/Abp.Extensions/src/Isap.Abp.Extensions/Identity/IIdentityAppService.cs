using System;
using System.Threading.Tasks;
using Isap.CommonCore.Services;

namespace Isap.Abp.Extensions.Identity
{
	public interface IIdentityAppService: ICommonAppService
	{
		Task<RoleDto> RoleExport(Guid roleId);
	}
}
