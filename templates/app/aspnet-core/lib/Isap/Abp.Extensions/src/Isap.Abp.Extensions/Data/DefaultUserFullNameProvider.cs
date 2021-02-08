using Isap.CommonCore.Utils;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Identity;

namespace Isap.Abp.Extensions.Data
{
	public class DefaultUserFullNameProvider: IUserFullNameProvider, ITransientDependency
	{
		public FullName GetFullName(IdentityUser user)
		{
			return new FullName(user.Surname, user.Name, null);
		}
	}
}
