using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Isap.Abp.BackgroundJobs.Jobs;
using Isap.Abp.BackgroundJobs.Queues;
using Isap.Abp.Extensions.Domain;
using Isap.CommonCore.Extensions;
using Isap.CommonCore.Utils;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;

namespace Isap.Abp.BackgroundJobs.EntityFrameworkCore
{
	public abstract class JobDataManagerBase: DomainManagerBase<IJobData, JobData, Guid, IRepository<JobData, Guid>>, IJobDataManager
	{
		protected IJobQueueCache JobQueueCache => LazyServiceProvider.LazyGetRequiredService<IJobQueueCache>();

		public async Task<IJobArguments> GetOrCreateArguments<TArgs>(TArgs args)
		{
			await Task.Yield();
			string key = GetKey(args, out string arguments);
			return new JobArguments
				{
					Key = key,
					ArgumentsType = typeof(TArgs).AssemblyQualifiedName,
					Arguments = JsonDocument.Parse(arguments, new JsonDocumentOptions
						{
							CommentHandling = JsonCommentHandling.Skip,
						}),
				};
		}

		public async Task<IJobData> CreateJob(Guid? tenantId, Guid queueId, string name, IJobArguments arguments, string concurrencyKey,
			BackgroundJobPriority priority,
			DateTime nextTryTime, CancellationToken cancellationToken = default)
		{
			using (DataFilter.Disable<IMultiTenant>())
			{
				DateTime now = Clock.Now;
				var jobData = new JobData(Guid.NewGuid())
					{
						TenantId = tenantId,
						CreationTime = now,
						QueueId = queueId,
						Name = name,
						ArgumentsKey = arguments.Key,
						ArgumentsType = arguments.ArgumentsTypeName,
						Arguments = arguments.Arguments,
						ConcurrencyKey = concurrencyKey,
						Priority = priority,
						State = JobStateType.Pending,
						TryCount = 0,
						NextTryTime = nextTryTime,
						LastTryTime = null,
						LockId = null,
						LockTime = null,
						Result = null,
					};
				IJobData data = await DataRepository.InsertAsync(jobData, true, cancellationToken);
				return data;
			}
		}

		public Task<IJobData> CreateJob(Guid? tenantId, Guid queueId, string name, IJobArguments arguments, string concurrencyKey,
			BackgroundJobPriority priority,
			TimeSpan? delay, CancellationToken cancellationToken = default)
		{
			DateTime now = Clock.Now;
			return CreateJob(tenantId, queueId, name, arguments, concurrencyKey, priority, now + delay ?? now, cancellationToken);
		}

		public async Task<IJobData> FindJob(Guid jobId, CancellationToken cancellationToken = default)
		{
			using (DataFilter.Disable<IMultiTenant>())
				return await FindJobInternal(jobId, cancellationToken);
		}

		public async Task<IJobData> FindJob(Guid? tenantId, Guid jobId, CancellationToken cancellationToken = default)
		{
			return await FindJobInternal(tenantId, jobId, cancellationToken);
		}

		public async Task<IJobData> FindJob<TArgs>(Guid? tenantId, string queueName, TArgs args, CancellationToken cancellationToken = default)
		{
			IJobQueueBase queue = await JobQueueCache.GetAsync(queueName, cancellationToken);
			if (queue == null)
				throw new KeyNotFoundException($"Queue with name {queueName} not found in cache!");

			return await FindJob(tenantId, queue.Id, args, cancellationToken);
		}

		public async Task<IJobData> FindJob(Guid? tenantId, Guid queueId, string argumentsKey, CancellationToken cancellationToken = default)
		{
			using (DataFilter.Disable<IMultiTenant>())
			{
				IJobData data = await CancellableQueryHelpers.Query(cToken =>
						DataRepository
							.FirstOrDefaultAsync(e => e.QueueId == queueId
									//&& e.JobType == jobType.AssemblyQualifiedName
									&& e.ArgumentsKey == argumentsKey
									&& e.State == JobStateType.Pending
									&& e.TenantId.HasValue == tenantId.HasValue
									&& (!e.TenantId.HasValue || e.TenantId == tenantId)
								, cToken)
					, cancellationToken: cancellationToken);

				return data;
			}
		}

		public async Task<IJobData> FindJob<TArgs>(Guid? tenantId, Guid queueId, TArgs args, CancellationToken cancellationToken = default)
		{
			string argumentsKey = GetKey(args, out _);
			IJobData jobData = await FindJob(tenantId, queueId, argumentsKey, cancellationToken);
			return jobData;
		}

		public async Task<(IJobData, IJobArguments)> FindJobWithArguments<TArgs>(Guid? tenantId, Guid queueId, TArgs args,
			CancellationToken cancellationToken = default)
		{
			IJobArguments arguments = await GetOrCreateArguments(args);
			IJobData jobData = await FindJob(tenantId, queueId, arguments.Key, cancellationToken);
			return (jobData, arguments);
		}

		public async Task<IJobData> Update(Guid jobId, Action<JobData> update, CancellationToken cancellationToken = default)
		{
			using (DataFilter.Disable<IMultiTenant>())
			{
				JobData data = await DataRepository.FirstOrDefaultAsync(e => e.Id == jobId, cancellationToken);
				if (data != null)
				{
					update(data);
					data = await DataRepository.UpdateAsync(data, true, cancellationToken);
				}

				return data;
			}
		}

		public async Task<bool> DeleteJob(Guid? tenantId, Guid jobId, CancellationToken cancellationToken = default)
		{
			using (DataFilter.Disable<IMultiTenant>())
			{
				var jobData = await FindJobInternal(tenantId, jobId, cancellationToken);
				if (jobData == null)
					return false;

				await DataRepository.DeleteAsync(jobData, true, cancellationToken);
				return true;
			}
		}

		public async Task<bool> DeleteJobs<TArgs>(Guid? tenantId, Guid queueId, TArgs args, CancellationToken cancellationToken = default)
		{
			using (DataFilter.Disable<IMultiTenant>())
			{
				string argumentsKey = GetKey(args, out _);
				List<JobData> jobsToDelete = await DataRepository
					.Where(e => e.QueueId == queueId
						//&& e.JobType == jobType
						&& e.ArgumentsKey == argumentsKey
						&& e.State == JobStateType.Pending
						&& e.TenantId.HasValue == tenantId.HasValue
						&& (!e.TenantId.HasValue || e.TenantId == tenantId)
						&& !e.LockId.HasValue
					)
					.ToListAsync(cancellationToken);
				if (jobsToDelete.Count == 0)
					return false;

				foreach (JobData jobData in jobsToDelete)
					await DataRepository.DeleteAsync(jobData, true, cancellationToken);

				return true;
			}
		}

		public async Task<IJobConcurrencyLock> GetConcurrencyLock(IJobData jobData, CancellationToken cancellationToken = default)
		{
			if (jobData.ConcurrencyKey.IsNullOrEmpty() || !jobData.LockId.HasValue) return null;
			return await GetOrCreateConcurrencyLock(jobData.TenantId, jobData.QueueId, jobData.ConcurrencyKey, jobData.LockId.Value, cancellationToken);
		}

		public async Task<bool> ReleaseConcurrencyLock(IJobData jobData, CancellationToken cancellationToken = default)
		{
			return await Perform(db => ReleaseConcurrencyLock(db, jobData, cancellationToken));
		}

		public virtual async Task<IJobData> DequeueJob(Guid queueId, Guid lockId, List<Guid> tenants, CancellationToken cancellationToken = default)
		{
			return await Perform(db => DequeueJob(db, queueId, lockId, tenants, cancellationToken));
		}

		public async Task ResetUncompletedJobs(DateTime resetBefore, CancellationToken cancellationToken = default)
		{
			await Perform(db => ResetUncompletedJobs(db, resetBefore, cancellationToken));
		}

		public async Task RemoveObsoleteJobs(DateTime removeBefore, CancellationToken cancellationToken = default)
		{
			await Perform(db => RemoveObsoleteJobs(db, removeBefore, cancellationToken));
		}

		protected override Expression<Func<JobData, bool>> CreateUniqueKeyPredicate(JobData entry)
		{
			return null;
		}

		private string GetKey<TArgs>(TArgs args, out string arguments)
		{
			arguments = JToken.FromObject(args).ToString(Formatting.None);
			string hash = $"{typeof(TArgs).AssemblyQualifiedName}|{arguments}".Md5Hash();
			int length = arguments.Length;
			return $"{hash}{length:x8}";
		}

		private async Task<JobData> FindJobInternal(Guid jobId, CancellationToken cancellationToken = default)
		{
			JobData jobData = await DataRepository.FirstOrDefaultAsync(e => e.Id == jobId, cancellationToken);
			return jobData;
		}

		private async Task<JobData> FindJobInternal(Guid? tenantId, Guid jobId, CancellationToken cancellationToken = default)
		{
			using (DataFilter.Disable<IMultiTenant>())
			{
				JobData jobData = await DataRepository
					.FirstOrDefaultAsync(e => e.Id == jobId && (e.TenantId ?? Guid.Empty) == (tenantId ?? Guid.Empty), cancellationToken);
				return jobData;
			}
		}

		protected async Task<IJobConcurrencyLock> GetOrCreateConcurrencyLock(Guid? tenantId, Guid queueId, string concurrencyKey, Guid lockId,
			CancellationToken cancellationToken = default)
		{
			return await Perform(db => GetOrCreateConcurrencyLock(db, tenantId, queueId, concurrencyKey, lockId, cancellationToken));
		}

		protected async Task<T> Perform<T>(Func<IBackgroundJobsDbContext, Task<T>> action)
		{
			using (var uow = UnitOfWorkManager.Begin(true))
			{
				IBackgroundJobsDbContext dbContext = (IBackgroundJobsDbContext) await DataRepository.GetDbContextAsync();
				T result = await action(dbContext);
				await uow.CompleteAsync();
				return result;
			}
		}

		protected abstract Task<IJobConcurrencyLock> GetOrCreateConcurrencyLock(IBackgroundJobsDbContext db,
			Guid? tenantId, Guid queueId, string concurrencyKey, Guid lockId, CancellationToken cancellationToken = default);

		protected abstract Task<bool> ReleaseConcurrencyLock(IBackgroundJobsDbContext db,
			IJobData jobData, CancellationToken cancellationToken = default);

		protected abstract Task<IJobData> DequeueJob(IBackgroundJobsDbContext db,
			Guid queueId, Guid lockId, List<Guid> tenants, CancellationToken cancellationToken = default);

		protected abstract Task<int> ResetUncompletedJobs(IBackgroundJobsDbContext db,
			DateTime resetBefore, CancellationToken cancellationToken = default);

		protected abstract Task<int> RemoveObsoleteJobs(IBackgroundJobsDbContext db,
			DateTime removeBefore, CancellationToken cancellationToken = default);
	}
}
