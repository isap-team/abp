using System;
using System.Text.Json;
using Isap.Abp.BackgroundJobs.Queues;
using Isap.CommonCore.Services;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.MultiTenancy;

namespace Isap.Abp.BackgroundJobs.Jobs
{
	public interface IJobData: IBackgroundProcessingEntity, IMultiTenant
	{
		Guid QueueId { get; }
		IJobQueue Queue { get; }

		string Name { get; }

		string ArgumentsKey { get; }
		Type ArgumentsType { get; }
		string ArgumentsTypeName { get; }
		JsonDocument Arguments { get; }

		string ConcurrencyKey { get; }

		BackgroundJobPriority Priority { get; }
		JobStateType State { get; }

		int TryCount { get; }
		DateTime NextTryTime { get; }
		DateTime? LastTryTime { get; }

		Guid? LockId { get; }
		DateTime? LockTime { get; }

		Type ResultType { get; }
		string ResultTypeName { get; }
		JsonDocument Result { get; }
	}

	public class JobData: BackgroundProcessingEntity<IJobData>, IJobData
	{
		public JobData()
		{
		}

		public JobData(Guid id)
			: base(id)
		{
		}

		public Guid? TenantId { get; set; }

		public Guid QueueId { get; set; }
		public JobQueue Queue { get; set; }
		IJobQueue IJobData.Queue => Queue;

		public string Name { get; set; }

		public string ArgumentsKey { get; set; }

		public string ArgumentsType { get; set; }

		Type IJobData.ArgumentsType => Type.GetType(ArgumentsType);
		string IJobData.ArgumentsTypeName => ArgumentsType;

		public JsonDocument Arguments { get; set; }

		public string ConcurrencyKey { get; set; }

		public BackgroundJobPriority Priority { get; set; }
		public JobStateType State { get; set; }

		public int TryCount { get; set; }
		public DateTime NextTryTime { get; set; }
		public DateTime? LastTryTime { get; set; }

		public Guid? LockId { get; set; }

		public DateTime? LockTime { get; set; }

		public string ResultType { get; set; }

		Type IJobData.ResultType => string.IsNullOrEmpty(ResultType) ? null : Type.GetType(ResultType);
		string IJobData.ResultTypeName => ResultType;

		public JsonDocument Result { get; set; }

		protected override void InternalAssign(IJobData source)
		{
			TenantId = source.TenantId;
			QueueId = source.QueueId;
			Name = source.Name;
			ArgumentsKey = source.ArgumentsKey;
			ArgumentsType = source.ArgumentsTypeName;
			Arguments = source.Arguments;
			ConcurrencyKey = source.ConcurrencyKey;
			Priority = source.Priority;
			State = source.State;
			TryCount = source.TryCount;
			NextTryTime = source.NextTryTime;
			LastTryTime = source.LastTryTime;
			LockId = source.LockId;
			LockTime = source.LockTime;
			ResultType = source.ResultTypeName;
			Result = source.Result;
		}

		object ICommonEntity.GetId() => Id;
	}
}
