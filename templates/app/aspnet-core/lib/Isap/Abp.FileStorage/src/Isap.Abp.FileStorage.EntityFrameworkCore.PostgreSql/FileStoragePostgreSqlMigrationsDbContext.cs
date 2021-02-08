using Isap.Abp.Extensions.EntityFrameworkCore;
using Isap.Abp.FileStorage.Files;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;

namespace Isap.Abp.FileStorage.EntityFrameworkCore.PostgreSql
{
	[ConnectionStringName(FileStorageDbProperties.ConnectionStringName)]
	public class FileStoragePostgreSqlMigrationsDbContext
		: IsapDbContext<FileStoragePostgreSqlMigrationsDbContext>, IFileStorageDbContext
	{
		public FileStoragePostgreSqlMigrationsDbContext(DbContextOptions<FileStoragePostgreSqlMigrationsDbContext> options)
			: base(options)
		{
			ModelBuilder = new FileStoragePostgreSqlModelBuilder();
		}

		public IFileStorageModelBuilder ModelBuilder { get; }

		public DbSet<FileData> FileData { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			ModelBuilder.OnModelCreating(modelBuilder, this);
		}
	}
}
