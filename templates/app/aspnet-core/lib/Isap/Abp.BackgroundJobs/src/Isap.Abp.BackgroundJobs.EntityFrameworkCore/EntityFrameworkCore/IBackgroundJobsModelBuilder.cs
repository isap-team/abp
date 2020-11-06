using Isap.Abp.Extensions.Data;

namespace Isap.Abp.BackgroundJobs.EntityFrameworkCore
{
	public interface IBackgroundJobsModelBuilder: IAbpModelBuilder<IBackgroundJobsDbContext, BackgroundJobsModelBuilderConfigurationOptions>
	{
	}
}
