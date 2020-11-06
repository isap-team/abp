using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Isap.Abp.Extensions.Clustering;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Volo.Abp.Uow;

namespace Isap.Abp.BackgroundJobs.EntityFrameworkCore.PostgreSql
{
	public class PostgreSqlJobQueueProcessorDataStore: JobQueueProcessorDataStoreBase
	{
		public IBackgroundJobsDbContext DbContext { get; set; }
		public ICurrentNode CurrentNode { get; set; }

		protected string TablePrefix => BackgroundJobsDbProperties.DbTablePrefix;
		protected string Schema => BackgroundJobsDbProperties.DbSchema ?? "public";

		public override async Task RegisterProcessorActivity(Guid queueId, Guid lockId, CancellationToken cancellationToken = default)
		{
			string sql = $@"
INSERT
INTO	{Schema}.""{TablePrefix}JobQueueProcessors""
	(	""Id""
	,	""CreationTime""
	,	""NodeId""
	,	""QueueId""
	,	""ApplicationRole""
	,	""LastActivityTime""
	)
VALUES
	(	@id
	,	@now
	,	@nodeId
	,	@queueId
	,	@appRole
	,	@now
	)
ON CONFLICT (""Id"")
DO
UPDATE
	SET	""LastActivityTime"" = @now
	;
";

			using (var uow = UnitOfWorkManager.Begin(new AbpUnitOfWorkOptions(), true))
			{
				await DbContext.Database
					.ExecuteSqlRawAsync(sql,
						new List<object>
							{
								new NpgsqlParameter<Guid>("@id", lockId),
								new NpgsqlParameter<DateTime>("@now", Clock.Now),
								new NpgsqlParameter<int>("@nodeId", CurrentNode.Id),
								new NpgsqlParameter<Guid>("@queueId", queueId),
								new NpgsqlParameter<string>("@appRole", CurrentNode.ApplicationRole),
							},
						cancellationToken);

				await uow.CompleteAsync(cancellationToken);
			}
		}
	}
}
