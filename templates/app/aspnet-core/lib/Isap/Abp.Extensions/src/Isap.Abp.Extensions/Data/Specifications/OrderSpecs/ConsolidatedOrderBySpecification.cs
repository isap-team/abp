using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Isap.Abp.Extensions.Data.Specifications.OrderSpecs
{
	public class ConsolidatedOrderBySpecification<TEntity>: OrderSpecificationBase<TEntity>, IEnumerable<IOrderSpecification<TEntity>>
	{
		private readonly ICollection<IOrderSpecification<TEntity>> _specifications;

		public ConsolidatedOrderBySpecification(ICollection<IOrderSpecification<TEntity>> specifications)
		{
			_specifications = specifications;
		}

		public override bool IsEmpty => _specifications.Count == 0;

		IEnumerator<IOrderSpecification<TEntity>> IEnumerable<IOrderSpecification<TEntity>>.GetEnumerator()
		{
			return _specifications.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _specifications.GetEnumerator();
		}

		public override IOrderedQueryable<TEntity> OrderBy(IQueryable<TEntity> query)
		{
			return IsEmpty
					? (IOrderedQueryable<TEntity>) query
					: _specifications.Skip(1).Aggregate(_specifications.First().OrderBy(query), (q, spec) => spec.ThenBy(q))
				;
		}

		public override IOrderedQueryable<TEntity> ThenBy(IOrderedQueryable<TEntity> query)
		{
			return IsEmpty
					? query
					: _specifications.Aggregate(query, (q, spec) => spec.ThenBy(q))
				;
		}

		public override IOrderedEnumerable<TEntity> OrderBy(IEnumerable<TEntity> query)
		{
			return IsEmpty
					? (IOrderedEnumerable<TEntity>) query
					: _specifications.Skip(1).Aggregate(_specifications.First().OrderBy(query), (q, spec) => spec.ThenBy(q))
				;
		}

		public override IOrderedEnumerable<TEntity> ThenBy(IOrderedEnumerable<TEntity> query)
		{
			throw new System.NotImplementedException();
		}

		public void Add(IOrderSpecification<TEntity> item)
		{
			_specifications.Add(item);
		}
	}
}
