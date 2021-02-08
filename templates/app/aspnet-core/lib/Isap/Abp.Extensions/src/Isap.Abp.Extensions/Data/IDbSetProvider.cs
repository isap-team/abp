using System.Linq;
using System.Threading.Tasks;

namespace Isap.Abp.Extensions.Data
{
	public interface IDbSetProvider
	{
		Task<IQueryable<TEntity>> GetDbSet<TEntity>() where TEntity: class;
	}
}
