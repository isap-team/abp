using Isap.CommonCore.Services;

namespace Isap.Abp.Extensions.Collections
{
	public interface IInMemoryDataStoreBuilder<in TKey, in TEntity>: IInMemoryDataCollectionBuilder<TKey, TEntity>
		where TEntity: ICommonEntity<TKey>
	{
	}
}
