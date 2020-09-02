using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Isap.Abp.Extensions.Domain;
using Isap.CommonCore.DependencyInjection;
using Isap.CommonCore.Services;
using JetBrains.Annotations;
using Microsoft.Extensions.Caching.Distributed;
using Volo.Abp;
using Volo.Abp.Caching;
using Volo.Abp.Domain.Entities;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Threading;
using Volo.Abp.Uow;

namespace Isap.Abp.Extensions.Caching
{
	public abstract class EntityCacheBase<TItemIntf, TItemImpl, TKey>: IEntityCache<TItemIntf, TKey>
		where TItemIntf: ICommonEntity<TKey>
		where TItemImpl: EntityCacheItemBase<TKey>, TItemIntf
	{
		public IDistributedCache<TItemImpl, TKey> Cache { get; set; }

		public abstract string EntityName { get; }

		[ItemNotNull]
		public TItemIntf Get(TKey id)
		{
			TItemIntf result = GetOrNull(id);
			if (result == null)
				throw new AbpException($"There is no {EntityName} with given id: {id}");
			return result;
		}

		[ItemNotNull]
		public async Task<TItemIntf> GetAsync(TKey id)
		{
			TItemIntf result = await GetOrNullAsync(id);
			if (result == null)
				throw new AbpException($"There is no {EntityName} with given id: {id}");
			return result;
		}

		[ItemCanBeNull]
		public TItemIntf GetOrNull(TKey id)
		{
			return AsyncHelper.RunSync(() => GetOrNullAsync(id));
		}

		[ItemCanBeNull]
		public async Task<TItemIntf> GetOrNullAsync(TKey id)
		{
			return await Cache.GetOrAddAsync(id, () => InternalGetOrNullAsync(id), GetCacheEntryOptions);
		}

		protected static DistributedCacheEntryOptions GetCacheEntryOptions()
		{
			return new DistributedCacheEntryOptions { SlidingExpiration = TimeSpan.FromSeconds(3600) };
		}

		protected abstract Task<TItemImpl> InternalGetOrNullAsync(TKey id);
	}

	public abstract class EntityCacheBase<TItemIntf, TItemImpl, TIntf, TImpl, TKey>
		: EntityCacheBase<TItemIntf, TItemImpl, TKey> /*, IEventHandler<EntityChangedEventData<TImpl>>, IShouldInitialize*/
		where TItemIntf: ICommonEntity<TKey>
		where TItemImpl: EntityCacheItemBase<TKey>, TItemIntf
		where TIntf: class, ICommonEntity<TKey>
		where TImpl: class, IEntity<TKey>, TIntf
	{
		[PropertyInject(IsOptional = false)]
		public IObjectMapper ObjectMapper { get; set; }

		[PropertyInject(IsOptional = false)]
		public IUnitOfWorkManager UnitOfWorkManager { get; set; }

		public override string EntityName => typeof(TImpl).Name;

		protected override Task<TItemImpl> InternalGetOrNullAsync(TKey id)
		{
			return GetOrNullAsync(e => ((IEntity<TKey>) e).Id.Equals(id));
		}

		protected virtual async Task<TItemImpl> GetOrNullAsync(Expression<Func<TImpl, bool>> predicate)
		{
			return MapToCacheItem(await WithUnitOfWork(uow => InternalGetOrNullAsync(predicate)));
		}

		protected virtual async Task<TResult> WithUnitOfWork<TResult>(Func<IUnitOfWork, Task<TResult>> action)
		{
			if (UnitOfWorkManager.Current != null)
				return await action(UnitOfWorkManager.Current);

			using (var unitOfWork = UnitOfWorkManager.Begin())
			{
				TResult result = await action(unitOfWork);
				await unitOfWork.CompleteAsync();
				return result;
			}
		}

		protected virtual TItemImpl MapToCacheItem(TIntf entry)
		{
			return ObjectMapper.Map<TIntf, TItemImpl>(entry);
		}

		protected abstract Task<TIntf> InternalGetOrNullAsync(Expression<Func<TImpl, bool>> predicate);
	}

	public abstract class EntityCacheBase<TItemIntf, TItemImpl, TIntf, TImpl, TKey, TDataStore>
		: EntityCacheBase<TItemIntf, TItemImpl, TIntf, TImpl, TKey> /*, IEventHandler<EntityChangedEventData<TImpl>>, IShouldInitialize*/
		where TItemIntf: ICommonEntity<TKey>
		where TItemImpl: EntityCacheItemBase<TKey>, TItemIntf
		where TIntf: class, ICommonEntity<TKey>
		where TImpl: class, IEntity<TKey>, TIntf
		where TDataStore: class, IReferenceDataStore<TIntf, TImpl, TKey>
	{
		[PropertyInject(IsOptional = false)]
		public TDataStore DomainManager { get; set; }

		protected override async Task<TIntf> InternalGetOrNullAsync(Expression<Func<TImpl, bool>> predicate)
		{
			return await DomainManager.GetSingle(predicate);
		}

		/*
		protected virtual string[] FiltersToDisable { get; } = new string[0];

		[PropertyInject(IsOptional = false)]
		public ICacheHelperProvider CacheHelperProvider { get; set; }

		[PropertyInject(IsOptional = false)]
		public IServiceBus ServiceBus { get; set; }



		void IEventHandler<EntityChangedEventData<TImpl>>.HandleEvent(EntityChangedEventData<TImpl> eventData)
		{
			Remove(eventData.Entity);
		}

		#region Implementation of IInitializable

		public void Initialize()
		{
			ICacheHelper cacheHelper = CacheHelperProvider.GetOrAddCacheHelper<Guid, TItemImpl>(CacheName);
			Debug.Assert(cacheHelper != null);
		}

		#endregion

		protected async Task<TItemImpl> GetOrNullAsync(Expression<Func<TImpl, bool>> predicate)
		{
			if (DomainManager.UnitOfWorkManager.Current == null)
			{
				using (IUnitOfWorkCompleteHandle handle = DomainManager.UnitOfWorkManager.Begin())
				{
					using (DomainManager.UnitOfWorkManager.Current.DisableFilter(FiltersToDisable))
					{
						TItemImpl result = await InternalGetOrNullAsync(predicate);
						await handle.CompleteAsync();
						return result;
					}
				}
			}

			using (DomainManager.UnitOfWorkManager.Current.DisableFilter(FiltersToDisable))
				return await InternalGetOrNullAsync(predicate);
		}

		protected virtual void Remove(TImpl entry)
		{
			GetCache().Remove(entry.Id);
			ServiceBus.Publish(new AbpEntityChangedEvent(CacheName, entry.Id.ToString()));
		}
		*/
	}
}
