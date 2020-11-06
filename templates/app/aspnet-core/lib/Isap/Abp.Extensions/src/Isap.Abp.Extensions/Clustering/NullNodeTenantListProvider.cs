using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Isap.Abp.Extensions.Clustering
{
	public class NullNodeTenantListProvider: INodeTenantListProvider, ISingletonDependency
	{
		#region Implementation of INodeTenantListProvider

		public Task<List<Guid>> GetCurrentNodeTenants()
		{
			return Task.FromResult<List<Guid>>(null);
		}

		#endregion
	}
}
