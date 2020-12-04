using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Isap.Abp.Extensions.Clustering;
using Microsoft.EntityFrameworkCore;
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
	(	{{0}}
	,	{{1}}
	,	{{2}}
	,	{{3}}
	,	{{4}}
	,	{{1}}
	)
ON CONFLICT (""Id"")
DO
UPDATE
	SET	""LastActivityTime"" = {{1}}
	;
";

			using (var uow = UnitOfWorkManager.Begin(new AbpUnitOfWorkOptions(), true))
			{
				await DbContext.Database
					.ExecuteSqlRawAsync(sql,
						new List<object>
							{
								lockId,
								Clock.Now,
								CurrentNode.Id,
								queueId,
								CurrentNode.ApplicationRole,
							},
						cancellationToken);

				await uow.CompleteAsync(cancellationToken);
			}
		}
	}
}
