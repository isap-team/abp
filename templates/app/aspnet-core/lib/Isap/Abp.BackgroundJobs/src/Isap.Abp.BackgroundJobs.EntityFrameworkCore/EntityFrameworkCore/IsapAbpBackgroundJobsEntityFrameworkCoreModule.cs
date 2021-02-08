using Isap.Abp.Extensions.Data;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace Isap.Abp.BackgroundJobs.EntityFrameworkCore
{
	[DependsOn(
		typeof(IsapAbpBackgroundJobsDomainModule),
		typeof(AbpEntityFrameworkCoreModule)
	)]
	public class IsapAbpBackgroundJobsEntityFrameworkCoreModule: AbpModule
	{
		public override void ConfigureServices(ServiceConfigurationContext context)
		{
			context.Services.AddTransient<IIsapDbContextProvider, IsapUnitOfWorkDbContextProvider<BackgroundJobsDbContext>>();

			context.Services.AddAbpDbContext<BackgroundJobsDbContext>(options =>
				{
					//options.AddRepository<BackgroundJobRecord, EfCoreBackgroundJobRepository>();
				});
		}
	}
}
