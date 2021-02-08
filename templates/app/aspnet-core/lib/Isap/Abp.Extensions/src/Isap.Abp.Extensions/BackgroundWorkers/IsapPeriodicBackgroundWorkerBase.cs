using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Threading;

namespace Isap.Abp.Extensions.BackgroundWorkers
{
	public abstract class IsapPeriodicBackgroundWorkerBase: AsyncPeriodicBackgroundWorkerBase
	{
		protected IsapPeriodicBackgroundWorkerBase(AbpAsyncTimer timer, IServiceScopeFactory serviceScopeFactory)
			: base(timer, serviceScopeFactory)
		{
		}
	}
}
