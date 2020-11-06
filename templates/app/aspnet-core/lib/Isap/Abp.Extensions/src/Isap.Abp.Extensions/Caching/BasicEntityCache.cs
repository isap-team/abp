using Isap.Abp.Extensions.Domain;
using Isap.CommonCore.Services;
using Volo.Abp.Domain.Entities;

namespace Isap.Abp.Extensions.Caching
{
	public class BasicEntityCache<TItemIntf, TItemImpl, TIntf, TImpl, TKey, TDomainManager>
		: EntityCacheBase<TItemIntf, TItemImpl, TIntf, TImpl, TKey, TDomainManager>
		where TItemIntf: ICommonEntity<TKey>
		where TItemImpl: EntityCacheItemBase<TKey>, TItemIntf
		where TIntf: class, ICommonEntity<TKey>
		where TImpl: class, IEntity<TKey>, TIntf
		where TDomainManager: class, IDomainManager<TIntf, TImpl, TKey>
	{
	}
}
