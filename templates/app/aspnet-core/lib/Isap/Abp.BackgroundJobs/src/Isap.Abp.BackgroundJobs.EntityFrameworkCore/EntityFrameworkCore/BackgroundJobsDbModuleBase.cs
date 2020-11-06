using Isap.Abp.Extensions.Data;

namespace Isap.Abp.BackgroundJobs.EntityFrameworkCore
{
	public abstract class BackgroundJobsDbModuleBase<TModelBuilderImpl>: AbpExtDbModule<IBackgroundJobsModelBuilder, TModelBuilderImpl>
		where TModelBuilderImpl: class, IBackgroundJobsModelBuilder
	{
	}
}
