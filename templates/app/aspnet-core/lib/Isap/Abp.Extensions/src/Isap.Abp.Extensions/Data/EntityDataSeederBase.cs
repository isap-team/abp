using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Isap.Abp.Extensions.Domain;
using Isap.CommonCore.Services;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace Isap.Abp.Extensions.Data
{
	public interface IEntityDataSeeder
	{
		bool IsForHost { get; }
		bool IsForTenant { get; }
		Task SeedAsync(DataSeedContext context);
	}

	public interface IEntityDataSeeder<TEntity, TKey>: IEntityDataSeeder
		where TEntity: class, IEntity<TKey>, ICommonEntity<TKey>, IAssignable<TKey, TEntity>
	{
	}

	public abstract class EntityDataSeederBase<TEntity, TKey>: DomainServiceBase, IEntityDataSeeder<TEntity, TKey>
		where TEntity: class, IEntity<TKey>, ICommonEntity<TKey>, IAssignable<TKey, TEntity>
	{
		protected IRepository<TEntity, TKey> Repository => LazyServiceProvider.LazyGetRequiredService<IRepository<TEntity, TKey>>();

		public abstract bool IsForHost { get; }
		public abstract bool IsForTenant { get; }

		public async Task SeedAsync(DataSeedContext context)
		{
			if ((!IsForHost || context.TenantId.HasValue) && (!IsForTenant || !context.TenantId.HasValue))
				return;

			TEntity[] entries = await InternalCreateStandardEntries(context);
			foreach (TEntity entry in entries)
			{
				await AddOrUpdate(entry);
			}

			await UnitOfWorkManager.Current.SaveChangesAsync();
		}

		protected abstract Expression<Func<TEntity, bool>> GetUniqueKeyPredicate(TEntity entry);

		protected abstract Task<TEntity[]> InternalCreateStandardEntries(DataSeedContext context);

		protected async Task AddOrUpdate(TEntity entry)
		{
			TEntity existingEntry = await Repository.FirstOrDefaultAsync(GetUniqueKeyPredicate(entry));
			if (existingEntry == null)
				await Repository.InsertAsync(entry);
			else
				existingEntry.Assign(entry);
		}
	}
}
