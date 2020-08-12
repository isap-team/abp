using System;
using Isap.Abp.BackgroundJobs.Jobs;
using Volo.Abp.MultiTenancy;

namespace Isap.Abp.BackgroundJobs.Logging
{
	public interface IJobExecutionLogEntry: IBackgroundProcessingEntity, IMultiTenant
	{
		Guid JobId { get; }
		IJobData Job { get; }

		JobStateType State { get; }
		Guid LockId { get; }
		DateTime StartTime { get; }
		DateTime EndTime { get; }
		string Log { get; }
	}

	public class JobExecutionLogEntry: BackgroundProcessingEntity<IJobExecutionLogEntry>, IJobExecutionLogEntry
	{
		public JobExecutionLogEntry()
		{
		}

		public JobExecutionLogEntry(Guid id)
			: base(id)
		{
		}

		public Guid? TenantId { get; set; }

		public Guid JobId { get; set; }
		public JobData Job { get; set; }
		IJobData IJobExecutionLogEntry.Job => Job;

		public JobStateType State { get; set; }
		public Guid LockId { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }
		public string Log { get; set; }

		protected override void InternalAssign(IJobExecutionLogEntry source)
		{
			TenantId = source.TenantId;
			JobId = source.JobId;
			State = source.State;
			LockId = source.LockId;
			StartTime = source.StartTime;
			EndTime = source.EndTime;
			Log = source.Log;
		}
	}
}
