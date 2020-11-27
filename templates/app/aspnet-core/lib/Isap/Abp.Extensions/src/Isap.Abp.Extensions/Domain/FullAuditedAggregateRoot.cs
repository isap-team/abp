using System;
using Isap.Abp.Extensions.Data;
using Isap.CommonCore.Services;
using JetBrains.Annotations;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Isap.Abp.Extensions.Domain
{
	public abstract class FullAuditedAggregateRoot<TKey, TIntf>: FullAuditedAggregateRoot<TKey>, IMultiTenantEntity<TKey>, IAssignable<TKey, TIntf>
		where TIntf: ICommonEntity<TKey>
	{
		protected FullAuditedAggregateRoot()
		{
		}

		protected FullAuditedAggregateRoot(TKey id)
			: base(id)
		{
		}

		public Guid? TenantId { get; set; }

		public virtual void Assign([NotNull] TIntf source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			if (source is IMultiTenant multiTenantSource)
				TenantId = multiTenantSource.TenantId;

			InternalAssign(source);
		}

		object ICommonEntity.GetId() => Id;

		protected abstract void InternalAssign(TIntf source);
	}
}
