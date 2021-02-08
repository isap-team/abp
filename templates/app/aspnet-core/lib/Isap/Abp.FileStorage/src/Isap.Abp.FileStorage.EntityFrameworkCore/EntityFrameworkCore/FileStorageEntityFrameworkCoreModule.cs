using Isap.Abp.Extensions.Data;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace Isap.Abp.FileStorage.EntityFrameworkCore
{
	[DependsOn(
		typeof(FileStorageDomainModule),
		typeof(AbpEntityFrameworkCoreModule)
	)]
	public class FileStorageEntityFrameworkCoreModule: AbpModule
	{
		public override void ConfigureServices(ServiceConfigurationContext context)
		{
			context.Services.AddTransient<IIsapDbContextProvider, IsapUnitOfWorkDbContextProvider<FileStorageDbContext>>();

			context.Services.AddAbpDbContext<FileStorageDbContext>(options =>
				{
					/* Add custom repositories here. Example:
					 * options.AddRepository<Question, EfCoreQuestionRepository>();
					 */
				});
		}
	}
}
