using Isap.Abp.BackgroundJobs.Jobs;
using Isap.Abp.BackgroundJobs.Logging;
using Isap.Abp.BackgroundJobs.Queues;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Isap.Abp.BackgroundJobs.EntityFrameworkCore
{
	[ConnectionStringName(BackgroundJobsDbProperties.ConnectionStringName)]
	public interface IBackgroundJobsDbContext: IEfCoreDbContext
	{
		DbSet<JobQueue> Queues { get; }
		DbSet<JobQueue> QueueProcessors { get; }
		DbSet<JobConcurrencyLock> ConcurrencyLocks { get; }
		DbSet<JobData> Jobs { get; }
		DbSet<JobExecutionLogEntry> ExecutionLog { get; }
	}
}
