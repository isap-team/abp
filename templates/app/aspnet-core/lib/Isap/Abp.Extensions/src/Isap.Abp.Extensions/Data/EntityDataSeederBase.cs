using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Isap.CommonCore.Services;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.Uow;

namespace Isap.Abp.Extensions.Data
{
	public interface IEntityDataSeeder
	{
		bool IsForTenant { get; }
		Task SeedAsync(DataSeedContext context);
	}

	public interface IEntityDataSeeder<TEntity, TKey>: IEntityDataSeeder
		where TEntity: class, IEntity<TKey>, ICommonEntity<TKey>, IAssignable<TKey, TEntity>
	{
	}

	public abstract class EntityDataSeederBase<TEntity, TKey>: IEntityDataSeeder<TEntity, TKey>
		where TEntity: class, IEntity<TKey>, ICommonEntity<TKey>, IAssignable<TKey, TEntity>
	{
		protected EntityDataSeederBase()
		{
			GuidGenerator = SimpleGuidGenerator.Instance;
		}

		public IGuidGenerator GuidGenerator { get; set; }
		public IRepository<TEntity, TKey> Repository { get; set; }
		public IUnitOfWorkManager UnitOfWorkManager { get; set; }

		public abstract bool IsForTenant { get; }

		public async Task SeedAsync(DataSeedContext context)
		{
			if (IsForTenant && !context.TenantId.HasValue || !IsForTenant && context.TenantId.HasValue)
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
