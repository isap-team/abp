using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Isap.Abp.Extensions.Domain;
using Isap.CommonCore.Services;
using Volo.Abp;
using Volo.Abp.Caching;
using Volo.Abp.Threading;

namespace Isap.Abp.Extensions.Caching
{
	public interface IDistributedEntityCache<TCacheItemIntf, in TKey>
		where TCacheItemIntf: class, ICommonEntity<TKey>
	{
		TCacheItemIntf Get(TKey id);
		Task<TCacheItemIntf> GetAsync(TKey id);
		TCacheItemIntf GetOrNull(TKey id);
		Task<TCacheItemIntf> GetOrNullAsync(TKey id);

		Task RemoveAsync(TKey id);
	}

	public abstract class DistributedEntityCacheBase<TCacheItemIntf, TCacheItemImpl, TKey>: IDistributedEntityCache<TCacheItemIntf, TKey>, ISupportsLazyServices
		where TCacheItemIntf: class, ICommonEntity<TKey>
		where TCacheItemImpl: class, ICommonEntityDto<TKey>, TCacheItemIntf
	{
		protected readonly object ServiceProviderLock = new object();

		protected abstract string EntityName { get; }

		public IServiceProvider ServiceProvider { get; set; }
		object ISupportsLazyServices.ServiceProviderLock => ServiceProviderLock;

		ConcurrentDictionary<Type, object> ISupportsLazyServices.ServiceReferenceMap { get; } = new ConcurrentDictionary<Type, object>();

		protected IDistributedCache<TCacheItemImpl, TKey> Cache => LazyGetRequiredService<IDistributedCache<TCacheItemImpl, TKey>>();

		public TCacheItemIntf Get(TKey id)
		{
			TCacheItemIntf result = GetOrNull(id);
			if (result == null)
				throw new AbpException($"There is no {EntityName} with given id: {id}");
			return result;
		}

		public async Task<TCacheItemIntf> GetAsync(TKey id)
		{
			TCacheItemIntf result = await GetOrNullAsync(id);
			if (result == null)
				throw new AbpException($"There is no {EntityName} with given id: {id}");
			return result;
		}

		public TCacheItemIntf GetOrNull(TKey id)
		{
			return AsyncHelper.RunSync(() => GetOrNullAsync(id));
		}

		public async Task<TCacheItemIntf> GetOrNullAsync(TKey id)
		{
			TCacheItemImpl result = await Cache.GetAsync(id);

			if (result == null)
			{
				result = await TryLoadItem(id);
				if (result != null)
				{
					// ReSharper disable once AccessToModifiedClosure
					result = Cache.GetOrAdd(id, () => result);
				}
			}

			return result;
		}

		public async Task RemoveAsync(TKey id)
		{
			await Cache.RemoveAsync(id, true, true);
		}

		protected TService LazyGetRequiredService<TService>()
		{
			return SupportsLazyServicesExtensions.LazyGetRequiredService<TService>(this);
		}

		protected abstract Task<TCacheItemImpl> TryLoadItem(TKey id);

		protected virtual async Task<TCacheItemIntf> InternalGetOrNullAsync<TIndexKey>(IDistributedCache<CacheRef<TCacheItemImpl, TKey>, TIndexKey> index, TIndexKey key,
			Func<TIndexKey, Task<TCacheItemImpl>> tryLoadItem)
		{
			CacheRef<TCacheItemImpl, TKey> cacheRef = await index.GetAsync(key);
			if (cacheRef == null)
			{
				TCacheItemImpl entry = await tryLoadItem(key);
				if (entry == null)
					return null;

				// ReSharper disable once AccessToModifiedClosure
				entry = Cache.GetOrAdd(((ICommonEntity<TKey>) entry).Id, () => entry);

				cacheRef = new CacheRef<TCacheItemImpl, TKey>(((ICommonEntity<TKey>) entry).Id);
				cacheRef = await index.GetOrAddAsync(key, () => Task.FromResult(cacheRef));
			}

			return await Cache.GetAsync(cacheRef.Id);
		}
	}

	public abstract class DistributedEntityCacheBase<TCacheItemIntf, TCacheItemImpl, TKey, TAppService>
		: DistributedEntityCacheBase<TCacheItemIntf, TCacheItemImpl, TKey>
		where TCacheItemIntf: class, ICommonEntity<TKey>
		where TCacheItemImpl: class, ICommonEntityDto<TKey>, TCacheItemIntf
		where TAppService: IReferenceAppService<TCacheItemImpl, TKey>
	{
		protected DistributedEntityCacheBase(TAppService appService)
		{
			AppService = appService;
		}

		public TAppService AppService { get; }

		protected override async Task<TCacheItemImpl> TryLoadItem(TKey id)
		{
			return await AppService.Get(id);
		}
	}
}
