using System;
using System.Threading;
using System.Threading.Tasks;
using Isap.Abp.BackgroundJobs.Configuration;
using Isap.Abp.BackgroundJobs.Jobs;
using Isap.Abp.BackgroundJobs.Processing;
using Isap.Abp.BackgroundJobs.Queues;
using Isap.Abp.Extensions.Clustering;
using Isap.Converters;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;

namespace Isap.Abp.BackgroundJobs
{
	public class ExtendedBackgroundJobManager: IExtendedBackgroundJobManager, ITransientDependency
	{
		public IBackgroundProcessingConfiguration Config { get; set; }
		public ICurrentNode CurrentNode { get; set; }
		public ICurrentTenant CurrentTenant { get; set; }
		public IJobDataManager JobDataManager { get; set; }
		public IJobQueueCache QueueCache { get; set; }
		public IUnitOfWorkManager UnitOfWorkManager { get; set; }
		public IValueConverter Converter { get; set; }
		public IJobQueueProcessorManager JobQueueProcessorManager { get; set; }

		public string DefaultQueueName => Config.DefaultQueueName;

		public Task<string> EnqueueAsync<TArgs>(TArgs args, BackgroundJobPriority priority = BackgroundJobPriority.Normal, TimeSpan? delay = null)
		{
			return EnqueueAsync(args, default, priority, delay);
		}

		public Task<string> EnqueueAsync<TArgs>(TArgs args, CancellationToken cancellationToken, BackgroundJobPriority priority = BackgroundJobPriority.Normal,
			TimeSpan? delay = null)
		{
			return EnqueueAsync(GetJobQueueName<TArgs>(), args, priority, delay, cancellationToken);
		}

		public Task<string> EnqueueAsync<TArgs>(TArgs args, string concurrencyKey, BackgroundJobPriority priority = BackgroundJobPriority.Normal,
			TimeSpan? delay = null, CancellationToken cancellationToken = default)
		{
			return EnqueueAsync(GetJobQueueName<TArgs>(), args, concurrencyKey, priority, delay, cancellationToken);
		}

		public Task<string> EnqueueAsync<TArgs>(string queueName, TArgs args, BackgroundJobPriority priority = BackgroundJobPriority.Normal,
			TimeSpan? delay = null, CancellationToken cancellationToken = default)
		{
			return EnqueueAsync(queueName, args, null, priority, delay, cancellationToken);
		}

		public async Task<string> EnqueueAsync<TArgs>(string queueName, TArgs args, string concurrencyKey,
			BackgroundJobPriority priority = BackgroundJobPriority.Normal,
			TimeSpan? delay = null, CancellationToken cancellationToken = default)
		{
			IJobQueueBase queue = await GetJobQueue(queueName);
			IJobArguments arguments = await JobDataManager.GetOrCreateArguments(args);
			IJobData jobData = await JobDataManager.CreateJob(CurrentTenant.Id, queue.Id, GetJobName<TArgs>(), arguments, concurrencyKey, priority, delay,
				cancellationToken);
			return jobData.Id.ToString();
		}

		public Task<string> EnqueueAsync<TArgs>(TArgs args, DateTime nextTryTime, BackgroundJobPriority priority = BackgroundJobPriority.Normal,
			CancellationToken cancellationToken = default)
		{
			return EnqueueAsync(GetJobQueueName<TArgs>(), args, null, nextTryTime, priority, cancellationToken);
		}

		public Task<string> EnqueueAsync<TArgs>(TArgs args, string concurrencyKey, DateTime nextTryTime,
			BackgroundJobPriority priority = BackgroundJobPriority.Normal,
			CancellationToken cancellationToken = default)
		{
			return EnqueueAsync(GetJobQueueName<TArgs>(), args, concurrencyKey, nextTryTime, priority, cancellationToken);
		}

		public Task<string> EnqueueAsync<TArgs>(string queueName, TArgs args, DateTime nextTryTime,
			BackgroundJobPriority priority = BackgroundJobPriority.Normal,
			CancellationToken cancellationToken = default)
		{
			return EnqueueAsync(queueName, args, null, nextTryTime, priority, cancellationToken);
		}

		public async Task<string> EnqueueAsync<TArgs>(string queueName, TArgs args, string concurrencyKey, DateTime nextTryTime,
			BackgroundJobPriority priority = BackgroundJobPriority.Normal, CancellationToken cancellationToken = default)
		{
			IJobQueueBase queue = await GetJobQueue(queueName);
			IJobArguments arguments = await JobDataManager.GetOrCreateArguments(args);
			IJobData jobData = await JobDataManager.CreateJob(CurrentTenant.Id, queue.Id, GetJobName<TArgs>(), arguments, concurrencyKey, priority, nextTryTime,
				cancellationToken);
			return jobData.Id.ToString();
		}

		public Task<string> FindAsync<TArgs>(TArgs args, CancellationToken cancellationToken = default)
		{
			return FindAsync(GetJobQueueName<TArgs>(), args, cancellationToken);
		}

		public async Task<string> FindAsync<TArgs>(string queueName, TArgs args, CancellationToken cancellationToken = default)
		{
			IJobQueueBase queue = await GetJobQueue(queueName);
			IJobData jobData = await JobDataManager.FindJob(CurrentTenant.Id, queue.Id, args, cancellationToken);
			return jobData?.Id.ToString();
		}

		public Task<string> EnsureAsync<TArgs>(TArgs args, BackgroundJobPriority priority = BackgroundJobPriority.Normal, TimeSpan? delay = null,
			CancellationToken cancellationToken = default)
		{
			return EnsureAsync(GetJobQueueName<TArgs>(), args, priority, delay, cancellationToken);
		}

		public Task<string> EnsureAsync<TArgs>(TArgs args, string concurrencyKey, BackgroundJobPriority priority = BackgroundJobPriority.Normal,
			TimeSpan? delay = null, CancellationToken cancellationToken = default)
		{
			return EnsureAsync(GetJobQueueName<TArgs>(), args, concurrencyKey, priority, delay, cancellationToken);
		}

		public Task<string> EnsureAsync<TArgs>(string queueName, TArgs args, BackgroundJobPriority priority = BackgroundJobPriority.Normal,
			TimeSpan? delay = null, CancellationToken cancellationToken = default)
		{
			return EnsureAsync(queueName, args, null, priority, delay, cancellationToken);
		}

		public async Task<string> EnsureAsync<TArgs>(string queueName, TArgs args, string concurrencyKey,
			BackgroundJobPriority priority = BackgroundJobPriority.Normal,
			TimeSpan? delay = null, CancellationToken cancellationToken = default)
		{
			if (UnitOfWorkManager.Current == null)
				throw new InvalidOperationException("Unit of work is not created yet.");

			IJobQueueBase queue = await GetJobQueue(queueName);
			if (queue == null)
				throw new InvalidOperationException($"Can't find queue with name = '{queueName}'.");

			(IJobData jobData, IJobArguments arguments) = await JobDataManager.FindJobWithArguments(CurrentTenant.Id, queue.Id, args, cancellationToken);
			if (jobData == null)
				jobData = await JobDataManager.CreateJob(CurrentTenant.Id, queue.Id, GetJobName<TArgs>(), arguments, concurrencyKey, priority, delay,
					cancellationToken);
			else
				jobData = await JobDataManager.Update(jobData.Id, e => e.ConcurrencyKey = e.ConcurrencyKey ?? concurrencyKey, cancellationToken);

			return jobData.Id.ToString();
		}

		public Task<string> EnsureAsync<TArgs>(TArgs args, DateTime nextTryTime, BackgroundJobPriority priority = BackgroundJobPriority.Normal,
			CancellationToken cancellationToken = default)
		{
			return EnsureAsync(GetJobQueueName<TArgs>(), args, null, nextTryTime, priority, cancellationToken);
		}

		public Task<string> EnsureAsync<TArgs>(TArgs args, string concurrencyKey, DateTime nextTryTime,
			BackgroundJobPriority priority = BackgroundJobPriority.Normal,
			CancellationToken cancellationToken = default)
		{
			return EnsureAsync(GetJobQueueName<TArgs>(), args, concurrencyKey, nextTryTime, priority, cancellationToken);
		}

		public Task<string> EnsureAsync<TArgs>(string queueName, TArgs args, DateTime nextTryTime,
			BackgroundJobPriority priority = BackgroundJobPriority.Normal,
			CancellationToken cancellationToken = default)
		{
			return EnsureAsync(queueName, args, null, nextTryTime, priority, cancellationToken);
		}

		public async Task<string> EnsureAsync<TArgs>(string queueName, TArgs args, string concurrencyKey, DateTime nextTryTime,
			BackgroundJobPriority priority = BackgroundJobPriority.Normal, CancellationToken cancellationToken = default)
		{
			IJobQueueBase queue = await GetJobQueue(queueName);
			(IJobData jobData, IJobArguments arguments) = await JobDataManager.FindJobWithArguments(CurrentTenant.Id, queue.Id, args, cancellationToken);
			if (jobData == null)
				jobData = await JobDataManager.CreateJob(CurrentTenant.Id, queue.Id, GetJobName<TArgs>(), arguments, concurrencyKey, priority, nextTryTime,
					cancellationToken);
			else
				jobData = await JobDataManager.Update(jobData.Id, e => e.ConcurrencyKey = e.ConcurrencyKey ?? concurrencyKey, cancellationToken);

			return jobData.Id.ToString();
		}

		public async Task<bool> DeleteAsync(string jobId, CancellationToken cancellationToken = default)
		{
			ConvertAttempt<Guid> attempt = Converter.TryConvertTo<Guid>(jobId);
			return attempt.IsSuccess && await DeleteAsync(attempt.Result, cancellationToken);
		}

		public Task<bool> DeleteAsync<TArgs>(TArgs args, CancellationToken cancellationToken = default)
		{
			return DeleteAsync(GetJobQueueName<TArgs>(), args, cancellationToken);
		}

		public async Task<bool> DeleteAsync<TArgs>(string queueName, TArgs args, CancellationToken cancellationToken = default)
		{
			IJobQueueBase queue = await GetJobQueue(queueName);
			return await JobDataManager.DeleteJobs(CurrentTenant.Id, queue.Id, args, cancellationToken);
		}

		public async Task<bool> CancelAsync(string jobId, CancellationToken cancellationToken = default)
		{
			ConvertAttempt<Guid> attempt = Converter.TryConvertTo<Guid>(jobId);
			return attempt.IsSuccess && await CancelAsync(attempt.Result, cancellationToken);
		}

		public async Task<bool> IsNewOrPendingAsync(string jobId, CancellationToken cancellationToken = default)
		{
			ConvertAttempt<Guid> attempt = Converter.TryConvertTo<Guid>(jobId);
			return attempt.IsSuccess && await IsNewOrPendingAsync(attempt.Result, cancellationToken);
		}

		protected async Task<IJobQueueBase> GetJobQueue(string queueName)
		{
			return await QueueCache.GetAsync(queueName);
		}

		protected string GetJobQueueName(Type jobArgsType)
		{
			return BackgroundJobAttribute.GetQueueName(jobArgsType) ?? DefaultQueueName;
		}

		protected string GetJobQueueName<TArgs>()
		{
			return GetJobQueueName(typeof(TArgs));
		}

		protected string GetJobName(Type jobArgsType)
		{
			return BackgroundJobAttribute.GetName(jobArgsType);
		}

		protected string GetJobName<TArgs>()
		{
			return BackgroundJobAttribute.GetName<TArgs>();
		}

		protected Task<bool> DeleteAsync(Guid jobId, CancellationToken cancellationToken = default)
		{
			return JobDataManager.DeleteJob(CurrentTenant.Id, jobId, cancellationToken);
		}

		protected async Task<bool> CancelAsync(Guid jobId, CancellationToken cancellationToken = default)
		{
			IJobData jobData = await JobDataManager.FindJob(CurrentTenant.Id, jobId, cancellationToken);

			if (jobData == null)
				return false;

			switch (jobData.State)
			{
				case JobStateType.Abandoned:
					return false;
				case JobStateType.Cancelled:
					return true;
			}

			await JobQueueProcessorManager.CancelJobIfRunning(jobId);

			jobData = await JobDataManager.FindJob(CurrentTenant.Id, jobId, cancellationToken);

			if (jobData.State == JobStateType.Completed || jobData.State == JobStateType.Abandoned)
				return false;

			await JobDataManager.Update(jobData.Id, j => j.State = JobStateType.Cancelled, cancellationToken);

			return true;
		}

		protected async Task<bool> IsNewOrPendingAsync(Guid jobId, CancellationToken cancellationToken = default)
		{
			IJobData jobData = await JobDataManager.FindJob(CurrentTenant.Id, jobId, cancellationToken);

			if (jobData == null)
				return false;

			switch (jobData.State)
			{
				case JobStateType.New:
					return true;
				case JobStateType.Pending:
					return true;
				default:
					return false;
			}
		}
	}
}
