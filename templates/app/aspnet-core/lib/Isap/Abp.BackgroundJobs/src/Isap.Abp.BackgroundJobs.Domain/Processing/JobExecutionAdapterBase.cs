using System.Threading;
using System.Threading.Tasks;

namespace Isap.Abp.BackgroundJobs.Processing
{
	public abstract class JobExecutionAdapterBase<TArgs>: IJobExecutionAdapter
	{
		public virtual Task<object> ExecuteAsync(object job, object args, CancellationToken cancellationToken)
		{
			return InternalExecuteAsync(job, (TArgs) args, cancellationToken);
		}

		protected abstract Task<object> InternalExecuteAsync(object job, TArgs args, CancellationToken cancellationToken);
	}
}
