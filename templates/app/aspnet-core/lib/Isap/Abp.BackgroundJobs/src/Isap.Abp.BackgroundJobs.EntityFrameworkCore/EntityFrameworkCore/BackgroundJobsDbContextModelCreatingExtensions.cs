using System;
using Isap.Abp.BackgroundJobs.Jobs;
using Isap.Abp.BackgroundJobs.Logging;
using Isap.Abp.BackgroundJobs.Processing;
using Isap.Abp.BackgroundJobs.Queues;
using Microsoft.EntityFrameworkCore;
using Volo.Abp;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Isap.Abp.BackgroundJobs.EntityFrameworkCore
{
	public static class BackgroundJobsDbContextModelCreatingExtensions
	{
		public static void ConfigureBackgroundJobs(
			this ModelBuilder modelBuilder,
			Action<BackgroundJobsModelBuilderConfigurationOptions> optionsAction = null)
		{
			Check.NotNull(modelBuilder, nameof(modelBuilder));

			var options = new BackgroundJobsModelBuilderConfigurationOptions(
				BackgroundJobsDbProperties.DbTablePrefix,
				BackgroundJobsDbProperties.DbSchema
			);

			optionsAction?.Invoke(options);

			modelBuilder.Entity<JobQueue>(b =>
				{
					b.ToTable(options.TablePrefix + "JobQueues", options.Schema);

					b.Property(e => e.Name).HasMaxLength(JobQueueConsts.MaxNameLength).IsRequired();

					b.HasIndex(e => e.Name).IsUnique();

					b.ConfigureByConvention();
				});

			modelBuilder.Entity<JobQueueProcessorData>(b =>
				{
					b.ToTable(options.TablePrefix + "JobQueueProcessors", options.Schema);

					b.Property(e => e.ApplicationRole).HasMaxLength(JobQueueProcessorConsts.MaxApplicationRoleLength).IsRequired();

					b.HasOne(e => e.Queue).WithMany().HasForeignKey(e => e.QueueId).OnDelete(DeleteBehavior.Cascade);

					b.ConfigureByConvention();
				});

			modelBuilder.Entity<JobConcurrencyLock>(b =>
				{
					b.ToTable(options.TablePrefix + "JobConcurrencyLocks", options.Schema);

					b.Property(e => e.ConcurrencyKey).HasMaxLength(JobConcurrencyLockConsts.MaxConcurrencyKeyLength).IsRequired();

					b.HasOne(e => e.Queue).WithMany().HasForeignKey(e => e.QueueId).OnDelete(DeleteBehavior.Cascade);

					b.HasIndex(e => new { e.TenantId, e.QueueId, e.ConcurrencyKey }).IsUnique();

					b.ConfigureByConvention();
				});

			modelBuilder.Entity<JobData>(b =>
				{
					b.ToTable(options.TablePrefix + "Jobs", options.Schema);

					b.Property(e => e.Name).HasMaxLength(JobDataConsts.MaxNameLength).IsRequired();
					b.Property(e => e.ArgumentsKey).HasMaxLength(JobDataConsts.MaxArgumentsKeyLength).IsRequired();
					b.Property(e => e.ArgumentsType).HasMaxLength(JobDataConsts.MaxArgumentsTypeLength).IsRequired();
					b.Property(e => e.Arguments).IsRequired();
					b.Property(e => e.ConcurrencyKey).HasMaxLength(JobDataConsts.MaxConcurrencyKeyLength);
					b.Property(e => e.Priority).HasDefaultValue(BackgroundJobPriority.Normal);
					b.Property(e => e.State).HasDefaultValue(JobStateType.New);
					b.Property(e => e.TryCount).HasDefaultValue(0);
					b.Property(e => e.ResultType).HasMaxLength(JobDataConsts.MaxResultTypeLength);
					b.Property(e => e.Result);

					b.HasOne(e => e.Queue).WithMany().HasForeignKey(e => e.QueueId).OnDelete(DeleteBehavior.Cascade);

					b.HasIndex(e => e.CreationTime);
					b.HasIndex(e => e.ArgumentsKey);
					b.HasIndex(e => e.LastTryTime).HasFilter("\"State\" = 2");
					b.HasIndex(e => e.QueueId).HasFilter("\"State\" = 1");

					b.ConfigureByConvention();
				});

			modelBuilder.Entity<JobExecutionLogEntry>(b =>
				{
					b.ToTable(options.TablePrefix + "JobExecutionLog", options.Schema);

					b.Property(e => e.Log).IsUnicode();

					b.HasOne(e => e.Job).WithMany().HasForeignKey(e => e.JobId).OnDelete(DeleteBehavior.Cascade);

					b.ConfigureByConvention();
				});
		}
	}
}
