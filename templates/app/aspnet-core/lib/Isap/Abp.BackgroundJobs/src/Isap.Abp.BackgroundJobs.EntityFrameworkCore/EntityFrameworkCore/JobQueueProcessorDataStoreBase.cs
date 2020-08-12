using System;
using System.Threading;
using System.Threading.Tasks;
using Isap.Abp.BackgroundJobs.Processing;
using Isap.Abp.Extensions.Domain;

namespace Isap.Abp.BackgroundJobs.EntityFrameworkCore
{
	public abstract class JobQueueProcessorDataStoreBase: DataStoreBase, IJobQueueProcessorDataStore
	{
		public abstract Task RegisterProcessorActivity(Guid queueId, Guid lockId, CancellationToken cancellationToken = default);
	}
}
