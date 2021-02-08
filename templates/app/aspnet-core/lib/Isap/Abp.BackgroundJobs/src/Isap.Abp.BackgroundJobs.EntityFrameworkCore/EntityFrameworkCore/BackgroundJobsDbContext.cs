using Isap.Abp.BackgroundJobs.Jobs;
using Isap.Abp.BackgroundJobs.Logging;
using Isap.Abp.BackgroundJobs.Queues;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Isap.Abp.BackgroundJobs.EntityFrameworkCore
{
	[ConnectionStringName(BackgroundJobsDbProperties.ConnectionStringName)]
	public class BackgroundJobsDbContext: AbpDbContext<BackgroundJobsDbContext>, IBackgroundJobsDbContext
	{
		public BackgroundJobsDbContext(DbContextOptions<BackgroundJobsDbContext> options)
			: base(options)
		{
		}

		public IBackgroundJobsModelBuilder ModelBuilder { get; set; }

		public DbSet<JobQueue> Queues { get; set; }
		public DbSet<JobQueue> QueueProcessors { get; set; }
		public DbSet<JobConcurrencyLock> ConcurrencyLocks { get; set; }
		public DbSet<JobData> Jobs { get; set; }
		public DbSet<JobExecutionLogEntry> ExecutionLog { get; set; }

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			ModelBuilder.OnModelCreating(builder, this);
		}
	}
}
