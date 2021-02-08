using System;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Isap.Abp.Extensions.Data
{
	public abstract class AbpModelBuilderBase<TDbContext, TModelBuilderConfigurationOptions>: IAbpModelBuilder<TDbContext, TModelBuilderConfigurationOptions>
		where TDbContext: IEfCoreDbContext
	{
		public abstract void OnModelCreating(ModelBuilder modelBuilder, TDbContext dbContext, Action<TModelBuilderConfigurationOptions> optionsAction = null);
	}
}
