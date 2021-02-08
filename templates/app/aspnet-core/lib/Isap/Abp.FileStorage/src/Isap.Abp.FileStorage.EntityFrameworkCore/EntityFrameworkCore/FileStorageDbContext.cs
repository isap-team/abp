using Isap.Abp.Extensions.EntityFrameworkCore;
using Isap.Abp.FileStorage.Files;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;

namespace Isap.Abp.FileStorage.EntityFrameworkCore
{
	[ConnectionStringName(FileStorageDbProperties.ConnectionStringName)]
	public class FileStorageDbContext: IsapDbContext<FileStorageDbContext>, IFileStorageDbContext
	{
		/* Add DbSet for each Aggregate Root here. Example:
		 * public DbSet<Question> Questions { get; set; }
		 */

		public FileStorageDbContext(DbContextOptions<FileStorageDbContext> options)
			: base(options)
		{
		}

		public IFileStorageModelBuilder ModelModelBuilder { get; set; }

		public DbSet<FileData> FileData { get; set; }

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			ModelModelBuilder.OnModelCreating(builder, this, options =>
				{
					options.TablePrefix = FileStorageDbProperties.DbTablePrefix;
					options.Schema = FileStorageDbProperties.DbSchema;
				});
		}
	}
}
