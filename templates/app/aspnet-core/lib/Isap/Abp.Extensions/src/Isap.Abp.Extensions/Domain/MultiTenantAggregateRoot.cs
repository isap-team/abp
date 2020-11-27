using System;
using Isap.CommonCore.Services;
using Volo.Abp.MultiTenancy;

namespace Isap.Abp.Extensions.Domain
{
	public interface IMultiTenantAggregateRoot<TKey>: ICommonAggregateRoot<TKey>, IMultiTenant
	{
	}

	public class MultiTenantAggregateRoot<TKey>: CommonAggregateRoot<TKey>, IMultiTenantAggregateRoot<TKey>, ICommonMultiTenant<Guid?>
	{
		public MultiTenantAggregateRoot()
		{
		}

		public MultiTenantAggregateRoot(TKey id)
			: base(id)
		{
		}

		public Guid? TenantId { get; set; }
	}
}
