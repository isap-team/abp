using System;
using System.Threading.Tasks;

namespace Isap.Abp.BackgroundJobs.Processing
{
	public interface IJobQueueProcessorManager
	{
		Task<bool> CancelJobIfRunning(Guid jobId);
	}
}
