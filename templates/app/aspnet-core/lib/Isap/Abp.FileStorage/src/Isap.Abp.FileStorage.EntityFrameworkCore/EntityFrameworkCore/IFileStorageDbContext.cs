using Isap.Abp.FileStorage.Files;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Isap.Abp.FileStorage.EntityFrameworkCore
{
	[ConnectionStringName(FileStorageDbProperties.ConnectionStringName)]
	public interface IFileStorageDbContext: IEfCoreDbContext
	{
		/* Add DbSet for each Aggregate Root here. Example:
		 * DbSet<Question> Questions { get; }
		 */

		DbSet<FileData> FileData { get; }
	}
}
