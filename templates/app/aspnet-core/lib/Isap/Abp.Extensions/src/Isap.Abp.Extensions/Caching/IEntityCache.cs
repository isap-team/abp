using System.Threading.Tasks;
using Isap.CommonCore.Services;

namespace Isap.Abp.Extensions.Caching
{
	public interface IEntityCache<TItem, in TKey>
		where TItem: ICommonEntity<TKey>
	{
		TItem Get(TKey id);
		Task<TItem> GetAsync(TKey id);
		TItem GetOrNull(TKey id);
		Task<TItem> GetOrNullAsync(TKey id);
	}
}
