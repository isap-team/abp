using Isap.Abp.Extensions.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Volo.Abp.Domain.Entities.Events;
using Volo.Abp.EntityFrameworkCore;

namespace Isap.Abp.Extensions.EntityFrameworkCore
{
	public abstract class IsapDbContext<TDbContext>: AbpDbContext<TDbContext>
		where TDbContext: DbContext
	{
		protected IsapDbContext(DbContextOptions<TDbContext> options)
			: base(options)
		{
		}

		protected override void ApplyAbpConcepts(EntityEntry entry, EntityChangeReport changeReport)
		{
			base.ApplyAbpConcepts(entry, changeReport);

			if (entry.Entity is IHasRevision versionedEntity)
			{
				versionedEntity.Revision = RevisionHelpers.GetNextRevision(Clock);
			}
		}
	}
}
