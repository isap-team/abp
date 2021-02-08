using System;
using Isap.Abp.Extensions.Data;
using Isap.CommonCore.Services;
using JetBrains.Annotations;
using Volo.Abp.MultiTenancy;

namespace Isap.Abp.Extensions.Domain
{
	// ReSharper disable once PossibleInterfaceMemberAmbiguity
	public interface IMultiTenantFullAuditedAggregateRoot<TKey>: ICommonFullAuditedAggregateRoot<TKey>, IMultiTenant, ICommonMultiTenant<Guid?>
	{
	}

	[Serializable]
	public abstract class MultiTenantFullAuditedAggregateRoot<TKey>: CommonFullAuditedAggregateRoot<TKey>, IMultiTenantFullAuditedAggregateRoot<TKey>
	{
		protected MultiTenantFullAuditedAggregateRoot()
		{
		}

		protected MultiTenantFullAuditedAggregateRoot(TKey id)
			: base(id)
		{
		}

		public Guid? TenantId { get; set; }
	}

	[Serializable]
	public abstract class MultiTenantFullAuditedAggregateRoot<TKey, TIntf>: MultiTenantFullAuditedAggregateRoot<TKey>, IAssignable<TKey, TIntf>
		where TIntf: ICommonEntity<TKey>
	{
		protected MultiTenantFullAuditedAggregateRoot()
		{
		}

		protected MultiTenantFullAuditedAggregateRoot(TKey id)
			: base(id)
		{
		}

		public virtual void Assign([NotNull] TIntf source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			if (source is IMultiTenant multiTenantSource)
				TenantId = multiTenantSource.TenantId;

			InternalAssign(source);
		}

		protected abstract void InternalAssign(TIntf source);
	}
}
