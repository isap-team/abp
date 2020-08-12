using System;
using System.Threading;
using System.Threading.Tasks;

namespace Isap.Abp.BackgroundJobs.Processing
{
	public interface IJobQueueProcessorDataStore
	{
		Task RegisterProcessorActivity(Guid queueId, Guid lockId, CancellationToken cancellationToken = default);
	}
}
