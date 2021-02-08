using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Isap.Abp.Extensions.Domain;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories;

namespace Isap.Abp.BackgroundJobs.Queues
{
	public interface IJobQueueManager: IDomainManager<IJobQueue, JobQueue, Guid>
	{
		Task<IJobQueue> GetQueue(string name, CancellationToken cancellationToken = default);
		Task<IJobQueue> EnsureQueue(string name, CancellationToken cancellationToken = default);
	}

	public class JobQueueManager: DomainManagerBase<IJobQueue, JobQueue, Guid, IRepository<JobQueue, Guid>>, IJobQueueManager
	{
		public async Task<IJobQueue> GetQueue(string name, CancellationToken cancellationToken = default)
		{
			return await DataRepository
				.Where(e => e.Name == name)
				.FirstOrDefaultAsync(cancellationToken);
		}

		public async Task<IJobQueue> EnsureQueue(string name, CancellationToken cancellationToken = default)
		{
			JobQueue jobQueue = await DataRepository
						.Where(e => e.Name == name)
						.FirstOrDefaultAsync(cancellationToken)
					?? await DataRepository.InsertAsync(new JobQueue(Guid.NewGuid()) { Name = name }, true, cancellationToken)
				;
			return jobQueue;
		}

		protected override Expression<Func<JobQueue, bool>> CreateUniqueKeyPredicate(JobQueue entry)
		{
			return e => e.Name == entry.Name;
		}
	}
}
