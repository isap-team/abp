using System.Linq;
using Isap.Abp.Extensions.Querying;

namespace Isap.Abp.Extensions.Data.Specifications.OrderSpecs
{
	public abstract class OrderSpecificationBase<TEntity>: IOrderSpecification<TEntity>
	{
		public abstract bool IsEmpty { get; }

		public abstract IOrderedQueryable<TEntity> OrderBy(IQueryable<TEntity> query);
		public abstract IOrderedQueryable<TEntity> ThenBy(IOrderedQueryable<TEntity> query);

		IQueryable<TEntity> IQueryableSortProvider<TEntity>.Apply(IQueryable<TEntity> query) => OrderBy(query);

		IQueryable IQueryableSortProvider.Apply(IQueryable query) => OrderBy((IQueryable<TEntity>) query);
	}
}
