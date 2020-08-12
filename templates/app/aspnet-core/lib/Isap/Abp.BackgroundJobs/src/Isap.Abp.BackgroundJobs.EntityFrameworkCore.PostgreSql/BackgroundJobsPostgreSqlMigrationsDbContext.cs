using Isap.Abp.BackgroundJobs.Jobs;
using Isap.Abp.BackgroundJobs.Logging;
using Isap.Abp.BackgroundJobs.Queues;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Isap.Abp.BackgroundJobs.EntityFrameworkCore.PostgreSql
{
	[ConnectionStringName(BackgroundJobsDbProperties.ConnectionStringName)]
	public class BackgroundJobsPostgreSqlMigrationsDbContext: AbpDbContext<BackgroundJobsPostgreSqlMigrationsDbContext>, IBackgroundJobsDbContext
	{
		public BackgroundJobsPostgreSqlMigrationsDbContext(DbContextOptions<BackgroundJobsPostgreSqlMigrationsDbContext> options)
			: base(options)
		{
			ModelBuilder = new BackgroundJobsPostgreSqlModelBuilder();
		}

		public IBackgroundJobsModelBuilder ModelBuilder { get; }

		public DbSet<JobQueue> Queues { get; set; }
		public DbSet<JobQueue> QueueProcessors { get; set; }
		public DbSet<JobConcurrencyLock> ConcurrencyLocks { get; set; }
		public DbSet<JobData> Jobs { get; set; }
		public DbSet<JobExecutionLogEntry> ExecutionLog { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			ModelBuilder.OnModelCreating(modelBuilder, this);
		}
	}
}
