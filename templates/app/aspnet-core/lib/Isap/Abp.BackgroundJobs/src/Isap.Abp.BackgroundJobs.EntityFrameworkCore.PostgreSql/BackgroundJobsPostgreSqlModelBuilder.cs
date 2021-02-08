using Isap.Abp.BackgroundJobs.Jobs;
using Microsoft.EntityFrameworkCore;

namespace Isap.Abp.BackgroundJobs.EntityFrameworkCore.PostgreSql
{
	public class BackgroundJobsPostgreSqlModelBuilder: BackgroundJobsModelBuilderBase
	{
		protected override void OnModelCreating(ModelBuilder modelBuilder, IBackgroundJobsDbContext dbContext,
			BackgroundJobsModelBuilderConfigurationOptions options)
		{
			modelBuilder.Entity<JobConcurrencyLock>(b =>
				{
					b.Property(e => e.TenantId).HasConversion(new GuidEmptyIfNullValueConverter());
				});

			modelBuilder.Entity<JobData>(b =>
				{
					b.Property(e => e.Arguments).HasColumnType("jsonb");
					b.Property(e => e.Result).HasColumnType("jsonb");
				});
		}
	}
}
