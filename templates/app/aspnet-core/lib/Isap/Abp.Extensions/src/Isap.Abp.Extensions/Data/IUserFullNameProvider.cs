using Isap.CommonCore.Utils;
using Volo.Abp.Identity;

namespace Isap.Abp.Extensions.Data
{
	public interface IUserFullNameProvider
	{
		FullName GetFullName(IdentityUser user);
	}
}
