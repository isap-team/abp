using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.BackgroundJobs;

namespace Isap.Abp.BackgroundJobs.Processing
{
	public class AsyncBackgroundJobExecutionAdapter<TArgs>: JobExecutionAdapterBase<TArgs>
	{
		protected override async Task<object> InternalExecuteAsync(object job, TArgs args, CancellationToken cancellationToken)
		{
			await Task.Factory.StartNew(() => ((IAsyncBackgroundJob<TArgs>) job).ExecuteAsync(args), cancellationToken);
			return null;
		}
	}
}
