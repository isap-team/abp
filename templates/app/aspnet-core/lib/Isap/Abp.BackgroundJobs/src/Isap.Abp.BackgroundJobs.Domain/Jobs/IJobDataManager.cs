using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Isap.Abp.Extensions.Domain;
using Volo.Abp.BackgroundJobs;

namespace Isap.Abp.BackgroundJobs.Jobs
{
	public interface IJobDataManager: IDomainManager<IJobData, JobData, Guid>
	{
		Task<IJobArguments> GetOrCreateArguments<TArgs>(TArgs args);

		Task<IJobData> CreateJob(Guid? tenantId, Guid queueId, string name, IJobArguments arguments, string concurrencyKey, BackgroundJobPriority priority,
			DateTime nextTryTime, CancellationToken cancellationToken = default);

		Task<IJobData> CreateJob(Guid? tenantId, Guid queueId, string name, IJobArguments arguments, string concurrencyKey, BackgroundJobPriority priority,
			TimeSpan? delay, CancellationToken cancellationToken = default);

		Task<IJobData> FindJob(Guid jobId, CancellationToken cancellationToken = default);
		Task<IJobData> FindJob(Guid? tenantId, Guid jobId, CancellationToken cancellationToken = default);
		Task<IJobData> FindJob(Guid? tenantId, Guid queueId, string argumentsKey, CancellationToken cancellationToken = default);
		Task<IJobData> FindJob<TArgs>(Guid? tenantId, Guid queueId, TArgs args, CancellationToken cancellationToken = default);
		Task<IJobData> FindJob<TArgs>(Guid? tenantId, string queueName, TArgs args, CancellationToken cancellationToken = default);
		Task<(IJobData, IJobArguments)> FindJobWithArguments<TArgs>(Guid? tenantId, Guid queueId, TArgs args, CancellationToken cancellationToken = default);

		Task<IJobData> Update(Guid jobId, Action<JobData> update, CancellationToken cancellationToken = default);
		Task<bool> DeleteJob(Guid? tenantId, Guid jobId, CancellationToken cancellationToken = default);
		Task<bool> DeleteJobs<TArgs>(Guid? tenantId, Guid queueId, TArgs args, CancellationToken cancellationToken = default);

		Task<IJobConcurrencyLock> GetConcurrencyLock(IJobData jobData, CancellationToken cancellationToken = default);
		Task<bool> ReleaseConcurrencyLock(IJobData jobData, CancellationToken cancellationToken = default);

		Task<IJobData> DequeueJob(Guid queueId, Guid lockId, List<Guid> tenants, CancellationToken cancellationToken = default);

		Task ResetUncompletedJobs(DateTime resetBefore, CancellationToken cancellationToken = default);
		Task RemoveObsoleteJobs(DateTime removeBefore, CancellationToken cancellationToken = default);
	}
}
