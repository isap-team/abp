using System;
using Isap.Abp.Extensions.Data;
using Isap.CommonCore.Services;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;

namespace Isap.Abp.BackgroundJobs
{
	public abstract class BackgroundProcessingEntity: AggregateRoot<Guid>, IHasCreationTime, IBackgroundProcessingEntity
	{
		protected BackgroundProcessingEntity()
		{
		}

		protected BackgroundProcessingEntity(Guid id)
			: base(id)
		{
		}

		public DateTime CreationTime { get; set; }

		object ICommonEntity.GetId() => Id;
	}

	public abstract class BackgroundProcessingEntity<TIntf>: BackgroundProcessingEntity, IAssignable<Guid, TIntf>
		where TIntf: IBackgroundProcessingEntity
	{
		protected BackgroundProcessingEntity()
		{
		}

		protected BackgroundProcessingEntity(Guid id)
			: base(id)
		{
		}

		public virtual void Assign(TIntf source)
		{
			InternalAssign(source);
		}

		protected abstract void InternalAssign(TIntf source);
	}
}
