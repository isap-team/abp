using System;
using Isap.Abp.Extensions.Data;
using Microsoft.EntityFrameworkCore;

namespace Isap.Abp.FileStorage.EntityFrameworkCore
{
	public interface IFileStorageModelBuilder: IAbpModelBuilder
	{
		void OnModelCreating(ModelBuilder modelBuilder, IFileStorageDbContext dbContext,
			Action<FileStorageModelBuilderConfigurationOptions> optionsAction = null);
	}
}
