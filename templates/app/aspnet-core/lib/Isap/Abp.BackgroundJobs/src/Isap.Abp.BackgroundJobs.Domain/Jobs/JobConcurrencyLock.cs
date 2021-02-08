using System;
using Isap.Abp.BackgroundJobs.Queues;
using Isap.CommonCore.Services;
using Volo.Abp.MultiTenancy;

namespace Isap.Abp.BackgroundJobs.Jobs
{
	public interface IJobConcurrencyLock: IBackgroundProcessingEntity, IMultiTenant
	{
		Guid QueueId { get; }
		IJobQueue Queue { get; }

		string ConcurrencyKey { get; }

		Guid LockId { get; }

		DateTime LockTime { get; }
	}

	public class JobConcurrencyLock: BackgroundProcessingEntity<IJobConcurrencyLock>, IJobConcurrencyLock
	{
		public JobConcurrencyLock()
		{
		}

		public JobConcurrencyLock(Guid id)
			: base(id)
		{
		}

		public Guid? TenantId { get; set; }

		public Guid QueueId { get; set; }
		public JobQueue Queue { get; set; }
		IJobQueue IJobConcurrencyLock.Queue => Queue;

		public string ConcurrencyKey { get; set; }

		public Guid LockId { get; set; }

		public DateTime LockTime { get; set; }

		object ICommonEntity.GetId() => Id;

		protected override void InternalAssign(IJobConcurrencyLock source)
		{
			TenantId = source.TenantId;
			QueueId = source.QueueId;
			ConcurrencyKey = source.ConcurrencyKey;
			LockId = source.LockId;
			LockTime = source.LockTime;
		}
	}
}
