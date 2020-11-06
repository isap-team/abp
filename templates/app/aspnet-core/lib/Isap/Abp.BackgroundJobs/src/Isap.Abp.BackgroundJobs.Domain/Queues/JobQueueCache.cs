using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Uow;

namespace Isap.Abp.BackgroundJobs.Queues
{
	public interface IJobQueueCache
	{
		Task<IJobQueueBase> GetAsync(string queueName, CancellationToken cancellationToken = default);
		Task<IJobQueueBase> GetOrCreateAsync(string queueName, CancellationToken cancellationToken = default);
	}

	public class JobQueueCache: IJobQueueCache, ITransientDependency
	{
		public IObjectMapper ObjectMapper { get; set; }
		public IJobQueueManager JobQueueManager { get; set; }
		public IUnitOfWorkManager UnitOfWorkManager { get; set; }
		public IDistributedCache<JobQueueDto> QueueCache { get; set; }

		public async Task<IJobQueueBase> GetAsync(string queueName, CancellationToken cancellationToken = default)
		{
			return await QueueCache.GetOrAddAsync(queueName, () => GetQueue(queueName, cancellationToken), token: cancellationToken);
		}

		public async Task<IJobQueueBase> GetOrCreateAsync(string queueName, CancellationToken cancellationToken = default)
		{
			return await QueueCache.GetOrAddAsync(queueName, () => EnsureQueue(queueName, cancellationToken), token: cancellationToken);
		}

		private async Task<JobQueueDto> GetQueue(string name, CancellationToken cancellationToken)
		{
			return await Perform(cancellationToken, async () =>
				{
					IJobQueue jobQueue = await JobQueueManager.GetQueue(name, cancellationToken);
					return ObjectMapper.Map<IJobQueue, JobQueueDto>(jobQueue);
				});
		}

		private async Task<JobQueueDto> EnsureQueue(string name, CancellationToken cancellationToken)
		{
			using (var uow = UnitOfWorkManager.Begin(true))
			{
				IJobQueue jobQueue = await JobQueueManager.EnsureQueue(name, cancellationToken);
				await uow.CompleteAsync(cancellationToken);
				return ObjectMapper.Map<IJobQueue, JobQueueDto>(jobQueue);
			}
		}

		private async Task<T> Perform<T>(CancellationToken cancellationToken, Func<Task<T>> action)
		{
			if (UnitOfWorkManager.Current == null)
			{
				using (var uow = UnitOfWorkManager.Begin())
				{
					T result = await action();
					await uow.CompleteAsync(cancellationToken);
					return result;
				}
			}

			return await action();
		}
	}
}
