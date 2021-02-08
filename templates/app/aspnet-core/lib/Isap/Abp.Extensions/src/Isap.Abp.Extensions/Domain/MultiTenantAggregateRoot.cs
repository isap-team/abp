using System;

namespace Isap.Abp.Extensions.Domain
{
	// ReSharper disable once PossibleInterfaceMemberAmbiguity
	[Obsolete("Use IMultiTenantFullAuditedAggregateRoot<TKey>")]
	public interface IMultiTenantAggregateRoot<TKey>: IMultiTenantFullAuditedAggregateRoot<TKey>
	{
	}

	[Serializable]
	[Obsolete("Use MultiTenantFullAuditedAggregateRoot<TKey>")]
	public class MultiTenantAggregateRoot<TKey>: MultiTenantFullAuditedAggregateRoot<TKey>, IMultiTenantAggregateRoot<TKey>
	{
		public MultiTenantAggregateRoot()
		{
		}

		public MultiTenantAggregateRoot(TKey id)
			: base(id)
		{
		}
	}
}
