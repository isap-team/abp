using Isap.CommonCore.Services;
using Volo.Abp.Application.Dtos;

namespace Isap.Abp.Extensions.Caching
{
	public abstract class EntityCacheItemBase<TKey>: EntityDto<TKey>, ICommonEntity<TKey>
	{
		public object GetId() => Id;
	}
}
