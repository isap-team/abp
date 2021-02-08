using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Isap.Abp.BackgroundJobs.Configuration;
using Isap.Abp.BackgroundJobs.Jobs;
using Isap.Abp.BackgroundJobs.Queues;
using Isap.Abp.Extensions;
using Isap.Abp.Extensions.Clustering;
using Isap.Abp.Extensions.Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Threading;
using Volo.Abp.Timing;

namespace Isap.Abp.BackgroundJobs.Processing
{
	[Volo.Abp.DependencyInjection.Dependency(ReplaceServices = true)]
	public class ExtendedBackgroundJobWorker: AsyncPeriodicBackgroundWorkerBase, IBackgroundJobWorker, IJobQueueProcessorManager
	{
		public const int MaxRecommendedBackgroundProcessingThreads = 20;

		/// <summary>
		///     Интервал запуска процедуры возврата "зависших" задач в очереди.
		/// </summary>
		internal const int ResetUncompletedJobsInterval = 10000;

		private readonly IBackgroundProcessingConfiguration _backgroundJobsConfig;
		private readonly List<IJobQueueProcessor> _queueProcessors;

		private int _timerCounter = -1;

		public ExtendedBackgroundJobWorker(
			AbpAsyncTimer timer,
			IOptions<AbpBackgroundJobsOptions> backgroundJobsOptions,
			IServiceScopeFactory serviceScopeFactory)
			: base(timer, serviceScopeFactory)
		{
			_backgroundJobsConfig = backgroundJobsOptions.Value;
			_queueProcessors = new List<IJobQueueProcessor>();
			Timer.Period = ResetUncompletedJobsInterval;
			Timer.RunOnStart = true;
		}

		protected IClock Clock => LazyServiceProvider.LazyGetRequiredService<IClock>();
		protected ICurrentNode CurrentNode => LazyServiceProvider.LazyGetRequiredService<ICurrentNode>();
		protected IJobQueueCache QueueCache => LazyServiceProvider.LazyGetRequiredService<IJobQueueCache>();
		protected IJobDataManager JobDataManager => LazyServiceProvider.LazyGetRequiredService<IJobDataManager>();
		protected IShutdownCancellationTokenProvider ShutdownCancellationTokenProvider => LazyServiceProvider.LazyGetRequiredService<IShutdownCancellationTokenProvider>();
		protected IJobQueueProcessorFactory JobQueueProcessorFactory => LazyServiceProvider.LazyGetRequiredService<IJobQueueProcessorFactory>();

		public override async Task StartAsync(CancellationToken cancellationToken = default)
		{
			await base.StartAsync(cancellationToken);

			await foreach ((IJobQueueBase queue, IJobQueueConfiguration queueConfig) in GetNodeQueues(cancellationToken))
			{
				_queueProcessors.AddRange(
					Enumerable.Repeat(queue, queueConfig.MaxThreadCount)
						.Select(q => JobQueueProcessorFactory.Create(q))
				);
			}

			if (_queueProcessors.Count > MaxRecommendedBackgroundProcessingThreads)
				Logger.LogWarning($"Too many background processing threads count = {_queueProcessors.Count}. "
					+ $"Recommended max background processing threads count is {MaxRecommendedBackgroundProcessingThreads}.");

			await Task.WhenAll(_queueProcessors.Select(p => p.StartAsync(cancellationToken)));
		}

		public override async Task StopAsync(CancellationToken cancellationToken = default)
		{
			await Task.WhenAll(_queueProcessors.Select(p => p.StopAsync(cancellationToken)));
			_queueProcessors.Clear();

			await base.StopAsync(cancellationToken);
		}

		public async Task<bool> CancelJobIfRunning(Guid jobId)
		{
			return await Task.WhenAll(_queueProcessors.Select(p => p.CancelJobIfRunning(jobId)))
					.ContinueWith(task => task.Result.Any(i => i))
				;
		}

		private async IAsyncEnumerable<(IJobQueueBase, IJobQueueConfiguration)> GetNodeQueues([EnumeratorCancellation] CancellationToken cancellationToken)
		{
			if (_backgroundJobsConfig.Queues == null)
				yield break;

			foreach (IJobQueueConfiguration config in _backgroundJobsConfig.Queues)
			{
				cancellationToken.ThrowIfCancellationRequested();

				if (config.ApplicationRoles.Contains(CurrentNode.ApplicationRole))
				{
					IJobQueueBase jobQueue = await QueueCache.GetOrCreateAsync(config.Name, cancellationToken);
					yield return (jobQueue, config);
				}
			}
		}

		protected override async Task DoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
		{
			if (!_backgroundJobsConfig.IsEnabled || ShutdownCancellationTokenProvider.CancellationToken.IsCancellationRequested) return;

			await JobDataManager.ResetUncompletedJobs(Clock.Now - _backgroundJobsConfig.ProcessorInactiveTimeout,
				ShutdownCancellationTokenProvider.CancellationToken);
			// Если обработчик таймера срабатывает раз в 10 секунд, то информацию о "старых" задачах удалять будем раз в 10 минут.
			if (Interlocked.Increment(ref _timerCounter) % 60 == 0)
				await JobDataManager.RemoveObsoleteJobs(Clock.Now - _backgroundJobsConfig.ObsoleteJobRemovingTimeout,
					ShutdownCancellationTokenProvider.CancellationToken);
		}
	}
}
