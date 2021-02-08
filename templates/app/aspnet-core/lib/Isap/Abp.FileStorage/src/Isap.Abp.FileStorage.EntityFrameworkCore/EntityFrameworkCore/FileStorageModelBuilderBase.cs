using System;
using Microsoft.EntityFrameworkCore;

namespace Isap.Abp.FileStorage.EntityFrameworkCore
{
	public abstract class FileStorageModelBuilderBase
	: IFileStorageModelBuilder
	{
		public void OnModelCreating(ModelBuilder modelBuilder, IFileStorageDbContext dbContext, Action<FileStorageModelBuilderConfigurationOptions> optionsAction = null)
		{
			var options = new FileStorageModelBuilderConfigurationOptions(
				FileStorageDbProperties.DbTablePrefix,
				FileStorageDbProperties.DbSchema
			);

			optionsAction?.Invoke(options);

			modelBuilder.ConfigureFileStorage(dbContext, o =>
				{
					o.TablePrefix = options.TablePrefix;
					o.Schema = options.Schema;
				});

			InternalModelCreating(modelBuilder, dbContext, options);
		}

		protected abstract void InternalModelCreating(ModelBuilder modelBuilder, IFileStorageDbContext dbContext,
			FileStorageModelBuilderConfigurationOptions options);
	}
}
