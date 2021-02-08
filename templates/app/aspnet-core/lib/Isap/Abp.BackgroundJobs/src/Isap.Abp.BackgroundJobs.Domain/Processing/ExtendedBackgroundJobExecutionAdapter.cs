using System.Threading;
using System.Threading.Tasks;

namespace Isap.Abp.BackgroundJobs.Processing
{
	public class ExtendedBackgroundJobExecutionAdapter<TArgs>: JobExecutionAdapterBase<TArgs>
	{
		protected override async Task<object> InternalExecuteAsync(object job, TArgs args, CancellationToken cancellationToken)
		{
			return await ((IExtendedAsyncBackgroundJob<TArgs>) job).ExecuteAsync(args, cancellationToken);
		}
	}
}
