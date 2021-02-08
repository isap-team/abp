using System;
using Isap.Abp.FileStorage.Files;
using Microsoft.EntityFrameworkCore;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Isap.Abp.FileStorage.EntityFrameworkCore
{
	public static class FileStorageDbContextModelCreatingExtensions
	{
		public static void ConfigureFileStorage(
			this ModelBuilder modelBuilder, IFileStorageDbContext dbContext,
			Action<FileStorageModelBuilderConfigurationOptions> optionsAction = null)
		{
			Check.NotNull(modelBuilder, nameof(modelBuilder));

			var options = new FileStorageModelBuilderConfigurationOptions(
				FileStorageDbProperties.DbTablePrefix,
				FileStorageDbProperties.DbSchema
			);

			optionsAction?.Invoke(options);

			/* Configure all entities here: */

			modelBuilder.Entity<FileData>(builder =>
				{
					builder.ToTable(options.TablePrefix + "FileData", options.Schema);
					//конфигурация базовых abp полей (если подключены к модели):
					builder.ConfigureByConvention();

					builder.Property(e => e.ContentType).HasMaxLength(64).IsRequired();
					builder.Property(e => e.Path).HasMaxLength(1024).IsRequired().IsUnicode();
					builder.Property(e => e.FileName).HasMaxLength(1024).IsRequired().IsUnicode();
					builder.Property(e => e.Name).HasMaxLength(128).IsRequired().IsUnicode();
					builder.Property(e => e.Hash).HasMaxLength(40).IsRequired();
					builder.Property(e => e.IsArchive);
					builder.Property(e => e.ArchiveTime);
				});
		}
	}
}
