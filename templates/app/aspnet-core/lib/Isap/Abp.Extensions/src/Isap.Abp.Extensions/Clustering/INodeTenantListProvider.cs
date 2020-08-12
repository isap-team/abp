using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Isap.Abp.Extensions.Clustering
{
	public interface INodeTenantListProvider
	{
		Task<List<Guid>> GetCurrentNodeTenants();
	}
}
