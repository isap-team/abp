using System.Threading;
using System.Threading.Tasks;

namespace Isap.Abp.BackgroundJobs.Processing
{
	public interface IJobExecutionAdapter
	{
		Task<object> ExecuteAsync(object job, object args, CancellationToken cancellationToken);
	}
}
