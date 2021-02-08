using System;

namespace Isap.Abp.Extensions.Domain
{
	[Obsolete("Use ICommonAggregateRoot<TKey>")]
	public interface ICommonAggregateRoot<TKey>: ICommonFullAuditedAggregateRoot<TKey>
	{
	}

	[Serializable]
	[Obsolete("Use CommonFullAuditedAggregateRoot<TKey>")]
	public abstract class CommonAggregateRoot<TKey>: CommonFullAuditedAggregateRoot<TKey>
	{
		protected CommonAggregateRoot()
		{
		}

		protected CommonAggregateRoot(TKey id)
			: base(id)
		{
		}
	}
}
