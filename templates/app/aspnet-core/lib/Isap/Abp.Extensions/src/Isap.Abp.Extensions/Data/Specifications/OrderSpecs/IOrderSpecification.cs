using System.Collections.Generic;
using System.Linq;
using Isap.Abp.Extensions.Querying;

namespace Isap.Abp.Extensions.Data.Specifications.OrderSpecs
{
	public interface IOrderSpecification<TEntity>: ISpecification<TEntity>, IQueryableSortProvider<TEntity>, IEnumerableSortProvider<TEntity>
	{
		bool IsEmpty { get; }

		IOrderedQueryable<TEntity> OrderBy(IQueryable<TEntity> query);
		IOrderedQueryable<TEntity> ThenBy(IOrderedQueryable<TEntity> query);

		IOrderedEnumerable<TEntity> OrderBy(IEnumerable<TEntity> query);
		IOrderedEnumerable<TEntity> ThenBy(IOrderedEnumerable<TEntity> query);
	}
}
