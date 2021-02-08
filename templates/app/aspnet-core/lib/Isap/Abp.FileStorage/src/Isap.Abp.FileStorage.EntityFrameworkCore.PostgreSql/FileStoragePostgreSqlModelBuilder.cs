using Isap.Abp.FileStorage.Files;
using Microsoft.EntityFrameworkCore;

namespace Isap.Abp.FileStorage.EntityFrameworkCore.PostgreSql
{
	public class FileStoragePostgreSqlModelBuilder
		: FileStorageModelBuilderBase
	{
		protected override void InternalModelCreating(ModelBuilder modelBuilder, IFileStorageDbContext dbContext, FileStorageModelBuilderConfigurationOptions options)
		{
			modelBuilder.Entity<FileData>()
				;
		}
	}
}
