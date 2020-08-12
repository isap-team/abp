using System;
using Isap.CommonCore.Services;

namespace Isap.Abp.Extensions.Caching
{
	public class CacheRef<TEntity, TKey>: ICommonEntityDto<TKey>
		where TEntity: class, ICommonEntityDto<TKey>
	{
		public CacheRef()
		{
		}

		public CacheRef(TKey id)
		{
			Id = id;
		}

		public TKey Id { get; set; }

		public Type GetEntityType() => typeof(TEntity);
	}
}
