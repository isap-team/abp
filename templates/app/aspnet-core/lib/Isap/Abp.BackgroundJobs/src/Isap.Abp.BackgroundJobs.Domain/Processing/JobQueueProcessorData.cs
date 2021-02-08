using System;
using Isap.Abp.BackgroundJobs.Queues;

namespace Isap.Abp.BackgroundJobs.Processing
{
	public interface IJobQueueProcessorData: IBackgroundProcessingEntity
	{
		int NodeId { get; }

		Guid QueueId { get; }
		IJobQueue Queue { get; }

		string ApplicationRole { get; }

		DateTime LastActivityTime { get; }
	}

	public class JobQueueProcessorData: BackgroundProcessingEntity<IJobQueueProcessorData>, IJobQueueProcessorData
	{
		public JobQueueProcessorData()
		{
		}

		public JobQueueProcessorData(Guid id)
			: base(id)
		{
		}

		public int NodeId { get; set; }

		public Guid QueueId { get; set; }
		public JobQueue Queue { get; set; }
		IJobQueue IJobQueueProcessorData.Queue => Queue;

		public string ApplicationRole { get; set; }

		public DateTime LastActivityTime { get; set; }

		protected override void InternalAssign(IJobQueueProcessorData source)
		{
			NodeId = source.NodeId;
			QueueId = source.QueueId;
			ApplicationRole = source.ApplicationRole;
			LastActivityTime = source.LastActivityTime;
		}
	}
}
