using System;
using Isap.CommonCore.Services;
using Volo.Abp.MultiTenancy;

namespace Isap.Abp.Extensions.Identity
{
	public interface IRoleBase: ICommonEntity<Guid>, IMultiTenant
	{
		string Name { get; }

		bool IsDefault { get; }
		bool IsStatic { get; }
		bool IsPublic { get; }
	}
}
