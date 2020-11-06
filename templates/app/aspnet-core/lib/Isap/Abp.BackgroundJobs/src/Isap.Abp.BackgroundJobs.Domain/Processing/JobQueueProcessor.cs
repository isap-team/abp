using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Isap.Abp.BackgroundJobs.Configuration;
using Isap.Abp.BackgroundJobs.Jobs;
using Isap.Abp.BackgroundJobs.Logging;
using Isap.Abp.Extensions;
using Isap.Abp.Extensions.Clustering;
using Isap.Abp.Extensions.Logging;
using Isap.CommonCore.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nito.AsyncEx;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Threading;
using Volo.Abp.Timing;
using Volo.Abp.Uow;
using ILoggerFactory = Castle.Core.Logging.ILoggerFactory;

namespace Isap.Abp.BackgroundJobs.Processing
{
	public interface IJobQueueProcessor: IRunnable
	{
		Task<bool> CancelJobIfRunning(Guid jobId);
	}

	public class JobQueueProcessor: AsyncPeriodicBackgroundWorkerBase, IJobQueueProcessor, ITransientDependency
	{
		protected class RunningJobInfo
		{
			private readonly CancellationTokenSource _cts = new CancellationTokenSource();
			private readonly CancellationTokenSource _linkedCancellationTokenSource;

			public RunningJobInfo(IJobData jobData, CancellationToken cancellationToken)
			{
				JobData = jobData;
				_linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_cts.Token, cancellationToken);
			}

			public IJobData JobData { get; }
			public CancellationToken CancellationToken => _linkedCancellationTokenSource.Token;

			public Task Task { get; set; }

			public async Task CancelAndWaitAsync(CancellationToken cancellationToken = default)
			{
				_cts.Cancel();
				await Task.WaitAsync(cancellationToken);
			}
		}

		public static TimeSpan UpdateActivityInterval = TimeSpan.FromSeconds(5);

		private readonly AbpTimer _activityTimer;
		private readonly IBackgroundProcessingConfiguration _backgroundJobsConfig;
		private RunningJobInfo _runningJob;
		private ILoggerFactory _castleLoggerFactory;

		public JobQueueProcessor(
			IOptions<AbpBackgroundJobOptions> jobOptions,
			IOptions<AbpBackgroundJobsOptions> backgroundJobsOptions,
			AbpTimer timer,
			AbpTimer activityTimer,
			IServiceScopeFactory serviceScopeFactory)
			: base(timer, serviceScopeFactory)
		{
			JobOptions = jobOptions.Value;
			_backgroundJobsConfig = backgroundJobsOptions.Value;

			_activityTimer = activityTimer;
			_activityTimer.RunOnStart = true;
			_activityTimer.Period = (int) UpdateActivityInterval.TotalMilliseconds;
			_activityTimer.Elapsed += ActivityTimerElapsed;

			Timer.RunOnStart = true;
		}

		protected AbpBackgroundJobOptions JobOptions { get; }

		public IClock Clock { get; set; }
		public IUnitOfWorkManager UnitOfWorkManager { get; set; }
		public IJobDataManager JobDataManager { get; set; }
		public IJobQueueProcessorDataStore JobQueueProcessorDataStore { get; set; }
		public IJobExecutionLogDataStore JobExecutionLogDataStore { get; set; }
		public INodeTenantListProvider NodeTenantListProvider { get; set; }
		public IShutdownCancellationTokenProvider ShutdownCancellationTokenProvider { get; set; }

		public Guid LockId { get; } = Guid.NewGuid();
		public IJobQueueBase Queue { get; private set; }
		public IJobQueueConfiguration QueueOptions { get; private set; }

		public ICurrentTenant CurrentTenant { get; set; }

		protected ILoggerFactory CastleLoggerFactory => LazyGetRequiredService(ref _castleLoggerFactory);

		public async Task<bool> CancelJobIfRunning(Guid jobId)
		{
			var job = GetRunningJob();
			if (job != null && job.JobData.Id == jobId)
			{
				await job.CancelAndWaitAsync();
				return true;
			}

			return false;
		}

		public void Attach(IJobQueueBase jobQueue)
		{
			Queue = jobQueue;
			QueueOptions = _backgroundJobsConfig.Queues.Single(i => i.Name == jobQueue.Name);
			Timer.Period = QueueOptions.JobPollInterval;
		}

		public override async Task StartAsync(CancellationToken cancellationToken = new CancellationToken())
		{
			await base.StartAsync(cancellationToken);
			_activityTimer.Start(cancellationToken);
		}

		public override Task StopAsync(CancellationToken cancellationToken = new CancellationToken())
		{
			_activityTimer.Stop(cancellationToken);
			return base.StopAsync(cancellationToken);
		}

		protected override async Task DoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
		{
			CancellationToken cancellationToken = ShutdownCancellationTokenProvider.CancellationToken;
			var logger = new MsToCastleLogger(CastleLoggerFactory, Logger, GetType());
			using (LoggingContext.Current.WithLogicalProperty(logger, "LoggingArea", "BackgroundProcessing"))
			using (LoggingContext.Current.WithLogicalProperty(logger, "LoggingSource", GetType().Name))
			using (LoggingContext.Current.WithLogicalProperty(logger, "LockId", LockId))
			using (LoggingContext.Current.WithLogicalProperty(logger, "JobQueueName", Queue.Name))
			{
				while (!cancellationToken.IsCancellationRequested)
				{
					List<Guid> tenants = await NodeTenantListProvider.GetCurrentNodeTenants();
					IJobData jobData = await JobDataManager.DequeueJob(Queue.Id, LockId, tenants, cancellationToken);
					if (jobData == null)
						return;

					var runningJobInfo = new RunningJobInfo(jobData, cancellationToken);

					lock (this)
						_runningJob = runningJobInfo;

					try
					{
						using (LoggingContext.Current.WithLogicalProperty(logger, "JobId", jobData.Id))
						using (LoggingContext.Current.WithLogicalProperty(logger, "TenantId", jobData.TenantId))
						using (var uow = UnitOfWorkManager.Begin(true, true))
						using (CurrentTenant.Change(jobData.TenantId))
						{
							await ExecuteAsync(workerContext, runningJobInfo, runningJobInfo.JobData, runningJobInfo.CancellationToken);
							await uow.CompleteAsync(cancellationToken);
						}
					}
					finally
					{
						lock (this) _runningJob = null;
					}
				}
			}
		}

		private void ActivityTimerElapsed(object sender, EventArgs e)
		{
			AsyncHelper.RunSync(() => JobQueueProcessorDataStore.RegisterProcessorActivity(Queue.Id, LockId));
		}

		protected RunningJobInfo GetRunningJob()
		{
			lock (this)
				return _runningJob;
		}

		private async Task ExecuteAsync(PeriodicBackgroundWorkerContext workerContext, RunningJobInfo runningJobInfo, IJobData jobData,
			CancellationToken cancellationToken)
		{
			ILogger logger = new MemoryLogger(Logger, Clock);

			try
			{
				Type argumentsType = jobData.ArgumentsType;

				if (argumentsType == null)
				{
					await TryUpdateAsync(jobData, e => e.State = JobStateType.Abandoned, cancellationToken);
					await JobExecutionLogDataStore.MakeExecutionLogEntry(jobData, $"Can't find job arguments type '{jobData.ArgumentsTypeName}'.");
					return;
				}

				JToken token = JToken.Parse(jobData.Arguments.RootElement.GetRawText());
				object arguments = token.ToObject(argumentsType);

				var jobExecutor = workerContext.ServiceProvider.GetRequiredService<IBackgroundJobExecuter>();
				var clock = workerContext.ServiceProvider.GetRequiredService<IClock>();

				jobData = await TryUpdateAsync(jobData, e =>
					{
						e.TryCount++;
						e.LastTryTime = clock.Now;
					}, cancellationToken);

				try
				{
					BackgroundJobConfiguration jobConfiguration = JobOptions.GetJob(jobData.Name);
					var context = new ExtendedJobExecutionContext(workerContext.ServiceProvider, jobConfiguration.JobType, arguments, logger, cancellationToken);

					try
					{
						AsyncFlowControl flowControl = ExecutionContext.SuppressFlow();
						try
						{
							IJobData localJobData = jobData;
							runningJobInfo.Task = Task.Run(() =>
								{
									using (CurrentTenant.Change(localJobData.TenantId))
										return jobExecutor.ExecuteAsync(context);
								}, CancellationToken.None);
						}
						finally
						{
							flowControl.Undo();
						}

						await runningJobInfo.Task;

						jobData = await TryUpdateAsync(jobData, e =>
							{
								e.State = JobStateType.Completed;
								e.ResultType = context.Result?.GetType().AssemblyQualifiedName;
								e.Result = context.Result == null ? null : JsonDocument.Parse(JToken.FromObject(context.Result).ToString(Formatting.None));
							}, cancellationToken);
					}
					catch (BackgroundJobExecutionException jobException)
					{
						logger.LogException(jobException.InnerException ?? jobException);

						var nextTryTime = CalculateNextTryTime(jobData, clock);

						jobData = await TryUpdateAsync(jobData, e =>
							{
								if (nextTryTime.HasValue)
								{
									e.NextTryTime = nextTryTime.Value;
								}
								else
								{
									e.State = JobStateType.Abandoned;
								}
							}, cancellationToken);
					}
				}
				catch (Exception ex)
				{
					logger.LogException(ex);
					Logger.LogWarning(
						$"Unexpected error occured while processing background job {jobData.Name} with arguments {jobData.Arguments}.",
						ex);
					jobData = await TryUpdateAsync(jobData, e => e.State = JobStateType.Abandoned, cancellationToken);
				}
			}
			catch (Exception exception)
			{
				logger.LogException(exception);
			}

			await JobExecutionLogDataStore.MakeExecutionLogEntry(jobData, logger.ToString());
		}

		protected virtual async Task<IJobData> TryUpdateAsync(IJobData jobData, Action<JobData> action, CancellationToken cancellationToken)
		{
			try
			{
				return await JobDataManager.Update(jobData.Id, action, cancellationToken);
			}
			catch (Exception updateEx)
			{
				Logger.LogException(updateEx);
				return jobData;
			}
		}

		protected virtual DateTime? CalculateNextTryTime(IJobData jobData, IClock clock)
		{
			var nextWaitDuration = QueueOptions.DefaultFirstWaitDuration * (Math.Pow(QueueOptions.DefaultWaitFactor, jobData.TryCount - 1));
			var nextTryDate = jobData.LastTryTime?.AddSeconds(nextWaitDuration) ??
				clock.Now.AddSeconds(nextWaitDuration);

			if (nextTryDate.Subtract(jobData.CreationTime).TotalSeconds > QueueOptions.DefaultTimeout)
			{
				return null;
			}

			return nextTryDate;
		}
	}
}
