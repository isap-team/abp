using System.Linq;

namespace Isap.Abp.Extensions.Data
{
	public interface IDbSetProvider
	{
		IQueryable<TEntity> GetDbSet<TEntity>() where TEntity: class;
	}
}
