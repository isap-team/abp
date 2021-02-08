using System;
using Isap.Abp.Extensions.Data;
using Isap.CommonCore.Services;
using JetBrains.Annotations;
using Volo.Abp.Domain.Entities.Auditing;

namespace Isap.Abp.Extensions.Domain
{
	[Serializable]
	public abstract class FullAuditedAggregateRoot<TKey, TIntf>: FullAuditedAggregateRoot<TKey>, IAssignable<TKey, TIntf>, ICommonEntity<TKey>
		where TIntf: ICommonEntity<TKey>
	{
		protected FullAuditedAggregateRoot()
		{
		}

		protected FullAuditedAggregateRoot(TKey id)
			: base(id)
		{
		}

		public virtual void Assign([NotNull] TIntf source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			InternalAssign(source);
		}

		object ICommonEntity.GetId() => Id;

		protected abstract void InternalAssign(TIntf source);
	}
}
