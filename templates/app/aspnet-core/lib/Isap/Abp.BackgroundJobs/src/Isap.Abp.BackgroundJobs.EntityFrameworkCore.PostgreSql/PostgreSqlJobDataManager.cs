using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Isap.Abp.BackgroundJobs.Jobs;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.MultiTenancy;

namespace Isap.Abp.BackgroundJobs.EntityFrameworkCore.PostgreSql
{
	public class PostgreSqlJobDataManager: JobDataManagerBase
	{
		public PostgreSqlJobDataManager(IRepository<JobData, Guid> dataRepository)
			: base(dataRepository)
		{
		}

		protected string TablePrefix => BackgroundJobsDbProperties.DbTablePrefix;
		protected string Schema => BackgroundJobsDbProperties.DbSchema ?? "public";

		protected override async Task<IJobConcurrencyLock> GetOrCreateConcurrencyLock(IBackgroundJobsDbContext db, Guid? tenantId, Guid queueId,
			string concurrencyKey, Guid lockId, CancellationToken cancellationToken = default)
		{
			if (concurrencyKey.IsNullOrEmpty()) return null;

			string sql = $@"
INSERT
INTO	{Schema}.""{TablePrefix}JobConcurrencyLocks""
	(	""Id""
	,	""TenantId""
	,	""QueueId""
	,	""ConcurrencyKey""
	,	""LockId""
	,	""LockTime""
	)
VALUES
	(	@id
	,	@tenantId
	,	@queueId
	,	@concurrencyKey
	,	@lockId
	,	@lockTime
	)
ON CONFLICT (""TenantId"", ""QueueId"", ""ConcurrencyKey"")
DO
NOTHING
	;
SELECT	""Id""
	,	""TenantId""
	,	""QueueId""
	,	""ConcurrencyKey""
	,	""LockId""
	,	""LockTime""
FROM	{Schema}.""{TablePrefix}JobConcurrencyLocks""
WHERE	NULL IS NULL
	AND	""TenantId"" = @tenantId
	AND	""QueueId"" = @queueId
	AND	""ConcurrencyKey"" = @concurrencyKey
	;
";
			IQueryable<JobConcurrencyLock> concurrencyLocks = db.Set<JobConcurrencyLock>()
				.FromSqlRaw(sql,
					new NpgsqlParameter<Guid>("@id", Guid.NewGuid()),
					new NpgsqlParameter<Guid>("@tenantId", tenantId ?? Guid.Empty),
					new NpgsqlParameter<Guid>("@queueId", queueId),
					new NpgsqlParameter<string>("@concurrencyKey", concurrencyKey),
					new NpgsqlParameter<Guid>("@lockId", lockId),
					new NpgsqlParameter<DateTime>("@lockTime", Clock.Now)
				);
			return await concurrencyLocks.FirstOrDefaultAsync(cancellationToken);
		}

		protected override async Task<bool> ReleaseConcurrencyLock(IBackgroundJobsDbContext db, IJobData jobData, CancellationToken cancellationToken = default)
		{
			if (jobData.ConcurrencyKey.IsNullOrEmpty() || !jobData.LockId.HasValue) return false;

			string sql = $@"
DELETE
FROM	{Schema}.""{TablePrefix}ConcurrencyLocks""
WHERE	NULL IS NULL
	AND	""TenantId"" = @tenantId
	AND	""QueueId"" = @queueId
	AND	""ConcurrencyKey"" = @concurrencyKey
	AND	""LockId"" = @lockId
";

			int rowCount = await db.Database
				.ExecuteSqlRawAsync(sql,
					new List<object>
						{
							new NpgsqlParameter<Guid>("@tenantId", jobData.TenantId ?? Guid.Empty),
							new NpgsqlParameter<Guid>("@queueId", jobData.QueueId),
							new NpgsqlParameter<string>("@concurrencyKey", jobData.ConcurrencyKey),
							new NpgsqlParameter<Guid>("@lockId", jobData.LockId.Value),
						},
					cancellationToken);
			return rowCount > 0;
		}

		protected override async Task<IJobData> DequeueJob(IBackgroundJobsDbContext db, Guid queueId, Guid lockId, List<Guid> tenants,
			CancellationToken cancellationToken = default)
		{
			string sql = $@"
UPDATE	{Schema}.""{TablePrefix}Jobs""
	SET	""LockId"" = @lockId
	,	""LockTime"" = @lockTime
WHERE	""Id"" =
		(
			SELECT	""Id""
			FROM	{Schema}.""{TablePrefix}Jobs""
			WHERE	NULL IS NULL
				AND	""QueueId"" = @queueId
				AND	""State"" = 1
				AND ""NextTryTime"" < @now
				AND	COALESCE(""LockId"", @lockId) = @lockId
				AND	COALESCE(""ConcurrencyKey"", '#####') NOT IN
					(	SELECT	l.""ConcurrencyKey""
						FROM	{Schema}.""{TablePrefix}JobConcurrencyLocks"" AS l
						WHERE	NULL IS NULL
							AND	l.""LockId"" != @lockId
							AND	(	@withSpecificTenants = FALSE
								OR	COALESCE(l.""TenantId"", '00000000-0000-0000-0000-000000000000'::uuid) = ANY(@tenants)
								)
					)
				AND	(	@withSpecificTenants = FALSE
					OR	COALESCE(""TenantId"", '00000000-0000-0000-0000-000000000000'::uuid) = ANY(@tenants)
					)
			ORDER
				BY	""Priority"" DESC
				,	""TryCount"" DESC
				,	""NextTryTime"" ASC
			LIMIT	1
			FOR UPDATE SKIP LOCKED
		)
	;
";
			DateTime lockTime = Clock.Now;
			using (DataFilter.Disable<IMultiTenant>())
				while (true)
				{
					int affectedRowsCount = await db.Database.ExecuteSqlRawAsync(sql, new object[]
						{
							new NpgsqlParameter<Guid>("@queueId", queueId),
							new NpgsqlParameter<Guid>("@lockId", lockId),
							new NpgsqlParameter<DateTime>("@lockTime", lockTime),
							new NpgsqlParameter<DateTime>("@now", Clock.Now),
							new NpgsqlParameter<bool>("@withSpecificTenants", tenants != null),
							new NpgsqlParameter<List<Guid>>("@tenants", tenants),
						}, cancellationToken);

					if (affectedRowsCount == 0)
						return null;

					JobData jobData = await db.Set<JobData>()
						.Where(e => e.State == JobStateType.Pending && e.LockId == lockId)
						.OrderByDescending(e => e.Priority)
						.FirstOrDefaultAsync(cancellationToken);

					if (jobData == null)
						return null;

					if (jobData.ConcurrencyKey.IsNullOrEmpty())
						return jobData;

					IJobConcurrencyLock existingLock =
						await GetOrCreateConcurrencyLock(db, jobData.TenantId, queueId, jobData.ConcurrencyKey, lockId, cancellationToken);

					for (int i = 0; i < 10 && existingLock == null; i++)
					{
						await Task.Delay(50, cancellationToken);
						existingLock = await GetOrCreateConcurrencyLock(db, jobData.TenantId, queueId, jobData.ConcurrencyKey, lockId, cancellationToken);
					}

					if (existingLock == null)
						throw new InvalidOperationException($"Can't get concurrency lock for job with id = '{jobData.Id}'.");

					if (existingLock.LockId == lockId)
						return jobData;

					await db.Database
						.ExecuteSqlRawAsync($@"UPDATE {Schema}.""{TablePrefix}Jobs"" SET ""LockId"" = @lockId WHERE ""Id"" = @jobId",
							new List<object>
								{
									new NpgsqlParameter<Guid>("@jobId", jobData.Id),
									new NpgsqlParameter<Guid>("@lockId", existingLock.LockId),
								},
							cancellationToken);
				}
		}

		protected override async Task<int> ResetUncompletedJobs(IBackgroundJobsDbContext db,
			DateTime resetBefore, CancellationToken cancellationToken = default)
		{
			string sql = $@"
UPDATE	{Schema}.""{TablePrefix}Jobs""
	SET	""LockId"" = null
	,	""LockTime"" = null
WHERE	""Id"" IN
		(
			SELECT	j.""Id""
			FROM	{Schema}.""{TablePrefix}Jobs"" AS j
					--INNER JOIN {Schema}.""{TablePrefix}JobQueues"" AS q
					--	ON	q.""Id"" = j.""QueueId""
					LEFT OUTER JOIN {Schema}.""{TablePrefix}JobQueueProcessors"" AS p
						ON	p.""QueueId"" = j.""QueueId""
						AND	p.""Id"" = j.""LockId""
			WHERE	NULL IS NULL
				AND	j.""LockId"" IS NOT NULL
				AND j.""State"" = 1
				--AND	COALESCE(NULLIF(q.""NodeId"", -1), NULLIF(p.""NodeId"", -1), -1) = COALESCE(NULLIF(p.""NodeId"", -1), NULLIF(q.""NodeId"", -1), -1)
				AND	COALESCE(p.""LastActivityTime"", j.""LockTime"") < @resetBefore
			FOR UPDATE OF j SKIP LOCKED
		)
	;

DELETE
FROM	{Schema}.""{TablePrefix}JobConcurrencyLocks""
WHERE	""Id"" IN
		(
			SELECT	j.""Id""
			FROM	{Schema}.""{TablePrefix}JobConcurrencyLocks"" AS j
					--INNER JOIN {Schema}.""{TablePrefix}JobQueues"" AS q
					--	ON	q.""Id"" = j.""QueueId""
					LEFT OUTER JOIN {Schema}.""{TablePrefix}JobQueueProcessors"" AS p
						ON	p.""QueueId"" = j.""QueueId""
						AND	p.""Id"" = j.""LockId""
			WHERE	NULL IS NULL
				--AND	COALESCE(NULLIF(q.""NodeId"", -1), NULLIF(p.""NodeId"", -1), -1) = COALESCE(NULLIF(p.""NodeId"", -1), NULLIF(q.""NodeId"", -1), -1)
				AND	COALESCE(p.""LastActivityTime"", j.""LockTime"") < @resetBefore
			FOR UPDATE OF j SKIP LOCKED
		)
	;

DELETE
FROM	{Schema}.""{TablePrefix}JobQueueProcessors""
WHERE	NULL IS NULL
	AND	""LastActivityTime"" < @resetBefore
	;
";
			return await db.Database
				.ExecuteSqlRawAsync(sql,
					new List<object>
						{
							new NpgsqlParameter<DateTime>("@resetBefore", resetBefore),
						},
					cancellationToken);
		}

		protected override async Task<int> RemoveObsoleteJobs(IBackgroundJobsDbContext db,
			DateTime removeBefore, CancellationToken cancellationToken = default)
		{
			string sql = $@"
DELETE
FROM	{Schema}.""{TablePrefix}JobExecutionLog""
USING	{Schema}.""{TablePrefix}Jobs"" AS j
WHERE	""JobId"" = j.""Id""
	AND	j.""State"" = 2
	AND	j.""LastTryTime"" < @removeBefore
;

DELETE
FROM	{Schema}.""{TablePrefix}Jobs"" AS j
WHERE	NULL IS NULL
	AND	j.""State"" = 2
	AND	j.""LastTryTime"" < @removeBefore
;
";
			return await db.Database
				.ExecuteSqlRawAsync(sql,
					new List<object>
						{
							new NpgsqlParameter<DateTime>("@removeBefore", removeBefore),
						},
					cancellationToken);
		}
	}
}
