using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.BackgroundJobs;

namespace Isap.Abp.BackgroundJobs.Processing
{
	public class BackgroundJobExecutionAdapter<TArgs>: JobExecutionAdapterBase<TArgs>
	{
		protected override async Task<object> InternalExecuteAsync(object job, TArgs args, CancellationToken cancellationToken)
		{
			await Task.Factory.StartNew(() => ((BackgroundJob<TArgs>) job).Execute(args), cancellationToken);
			return null;
		}
	}
}
