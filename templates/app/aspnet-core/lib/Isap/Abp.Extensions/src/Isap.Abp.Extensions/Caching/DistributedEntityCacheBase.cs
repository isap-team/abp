using System;
using System.Threading.Tasks;
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
	}

	public abstract class DistributedEntityCacheBase<TCacheItemIntf, TCacheItemImpl, TKey>: IDistributedEntityCache<TCacheItemIntf, TKey>
		where TCacheItemIntf: class, ICommonEntity<TKey>
		where TCacheItemImpl: class, TCacheItemIntf
	{
		protected abstract string EntityName { get; }

		public IDistributedCache<TCacheItemImpl, TKey> Cache { get; set; }

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

		protected abstract Task<TCacheItemImpl> TryLoadItem(TKey id);
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

		protected virtual async Task<TCacheItemIntf> InternalGetOrNullAsync(IDistributedCache<CacheRef<TCacheItemImpl, TKey>, string> index, string name,
			Func<string, Task<TCacheItemImpl>> tryLoadItem)
		{
			CacheRef<TCacheItemImpl, TKey> cacheRef = await index.GetAsync(name);
			if (cacheRef == null)
			{
				TCacheItemImpl entry = await tryLoadItem(name);
				if (entry == null)
					return null;

				// ReSharper disable once AccessToModifiedClosure
				entry = Cache.GetOrAdd(((ICommonEntity<TKey>) entry).Id, () => entry);

				cacheRef = new CacheRef<TCacheItemImpl, TKey>(((ICommonEntity<TKey>) entry).Id);
				cacheRef = await index.GetOrAddAsync(name, () => Task.FromResult(cacheRef));
			}

			return await Cache.GetAsync(cacheRef.Id);
		}
	}
}
