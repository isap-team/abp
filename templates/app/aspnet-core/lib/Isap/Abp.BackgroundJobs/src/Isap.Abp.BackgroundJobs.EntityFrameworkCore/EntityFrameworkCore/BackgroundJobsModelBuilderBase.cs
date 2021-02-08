using System;
using Isap.Abp.Extensions.Data;
using Microsoft.EntityFrameworkCore;

namespace Isap.Abp.BackgroundJobs.EntityFrameworkCore
{
	public abstract class BackgroundJobsModelBuilderBase
		: AbpModelBuilderBase<IBackgroundJobsDbContext, BackgroundJobsModelBuilderConfigurationOptions>, IBackgroundJobsModelBuilder
	{
		public override void OnModelCreating(ModelBuilder modelBuilder, IBackgroundJobsDbContext dbContext,
			Action<BackgroundJobsModelBuilderConfigurationOptions> optionsAction = null)
		{
			var options = new BackgroundJobsModelBuilderConfigurationOptions(
				BackgroundJobsDbProperties.DbTablePrefix,
				BackgroundJobsDbProperties.DbSchema
			);

			optionsAction?.Invoke(options);

			modelBuilder.ConfigureBackgroundJobs(o =>
				{
					o.TablePrefix = options.TablePrefix;
					o.Schema = options.Schema;
				});

			OnModelCreating(modelBuilder, dbContext, options);
		}

		protected abstract void OnModelCreating(ModelBuilder modelBuilder, IBackgroundJobsDbContext dbContext,
			BackgroundJobsModelBuilderConfigurationOptions options);
	}
}
