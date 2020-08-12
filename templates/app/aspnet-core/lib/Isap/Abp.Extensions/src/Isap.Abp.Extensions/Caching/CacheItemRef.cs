using System;
using Isap.CommonCore.Services;

namespace Isap.Abp.Extensions.Caching
{
	public class CacheItemRef<TEntity, TKey>: ICommonEntity<TKey>
		where TEntity: class, ICommonEntity<TKey>
	{
		public CacheItemRef(TKey id)
		{
			Id = id;
		}

		public CacheItemRef()
		{
		}

		public TKey Id { get; set; }

		object ICommonEntity.GetId() => Id;

		public Type GetEntityType() => typeof(TEntity);
	}
}
