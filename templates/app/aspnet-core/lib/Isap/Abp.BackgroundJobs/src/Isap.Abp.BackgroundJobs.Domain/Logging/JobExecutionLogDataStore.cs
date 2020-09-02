using System;
using System.Linq;
using System.Threading.Tasks;
using Isap.Abp.BackgroundJobs.Jobs;
using Isap.Abp.Extensions.Domain;
using Volo.Abp.Domain.Repositories;

namespace Isap.Abp.BackgroundJobs.Logging
{
	public interface IJobExecutionLogDataStore: IReferenceDataStore<IJobExecutionLogEntry, JobExecutionLogEntry, Guid>
	{
		Task<IJobExecutionLogEntry> MakeExecutionLogEntry(IJobData jobData, string log);
	}

	public class JobExecutionLogDataStore: ReferenceDataStoreBase<IJobExecutionLogEntry, JobExecutionLogEntry, Guid>, IJobExecutionLogDataStore
	{
		protected IRepository<JobExecutionLogEntry, Guid> DataRepository => LazyGetRequiredService<IRepository<JobExecutionLogEntry, Guid>>();

		public async Task<IJobExecutionLogEntry> MakeExecutionLogEntry(IJobData jobData, string log)
		{
			DateTime now = Clock.Now;
			JobExecutionLogEntry entry = await DataRepository.InsertAsync(new JobExecutionLogEntry(Guid.NewGuid())
				{
					TenantId = CurrentTenant.Id,
					CreationTime = now,
					JobId = jobData.Id,
					State = jobData.State,
					LockId = jobData.LockId ?? Guid.Empty,
					StartTime = jobData.LastTryTime ?? jobData.CreationTime,
					EndTime = now,
					Log = log,
				});
			return entry;
		}

		protected override IQueryable<JobExecutionLogEntry> GetQuery()
		{
			return DataRepository;
		}
	}
}
