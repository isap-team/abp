using System;
using Isap.CommonCore.Services;

namespace Isap.Abp.BackgroundJobs.Queues
{
	public interface IJobQueue: IJobQueueBase
	{
	}

	public class JobQueue: BackgroundProcessingEntity<IJobQueue>, IJobQueue
	{
		public JobQueue()
		{
		}

		public JobQueue(Guid id)
			: base(id)
		{
		}

		public string Name { get; set; }

		object ICommonEntity.GetId() => Id;

		protected override void InternalAssign(IJobQueue source)
		{
			Name = source.Name;
		}
	}
}
