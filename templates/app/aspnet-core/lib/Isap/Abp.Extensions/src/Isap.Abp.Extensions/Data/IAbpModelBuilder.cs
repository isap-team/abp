using System;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Isap.Abp.Extensions.Data
{
	public interface IAbpModelBuilder
	{
	}

	public interface IAbpModelBuilder<in TDbContext, out TModelBuilderConfigurationOptions>: IAbpModelBuilder
		where TDbContext: IEfCoreDbContext
	{
		void OnModelCreating(ModelBuilder modelBuilder, TDbContext dbContext, Action<TModelBuilderConfigurationOptions> optionsAction = null);
	}
}
