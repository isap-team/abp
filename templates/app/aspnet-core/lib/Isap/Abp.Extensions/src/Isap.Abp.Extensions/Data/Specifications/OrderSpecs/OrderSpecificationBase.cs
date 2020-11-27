using System.Collections;
using System.Collections.Generic;
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

		public abstract IOrderedEnumerable<TEntity> OrderBy(IEnumerable<TEntity> query);
		public abstract IOrderedEnumerable<TEntity> ThenBy(IOrderedEnumerable<TEntity> query);

		IEnumerable<TEntity> IEnumerableSortProvider<TEntity>.Apply(IEnumerable<TEntity> query) => OrderBy(query);

		IEnumerable IEnumerableSortProvider.Apply(IEnumerable query) => OrderBy(query.Cast<TEntity>());
	}
}
