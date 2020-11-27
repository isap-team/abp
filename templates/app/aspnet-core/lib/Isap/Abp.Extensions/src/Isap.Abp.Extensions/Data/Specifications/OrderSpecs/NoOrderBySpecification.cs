using System.Collections.Generic;
using System.Linq;

namespace Isap.Abp.Extensions.Data.Specifications.OrderSpecs
{
	public class NoOrderBySpecification<TEntity>: OrderSpecificationBase<TEntity>
	{
		public override bool IsEmpty => true;

		public override IOrderedQueryable<TEntity> OrderBy(IQueryable<TEntity> query)
		{
			return (IOrderedQueryable<TEntity>) query;
		}

		public override IOrderedQueryable<TEntity> ThenBy(IOrderedQueryable<TEntity> query)
		{
			return query;
		}

		public override IOrderedEnumerable<TEntity> OrderBy(IEnumerable<TEntity> query)
		{
			return (IOrderedEnumerable<TEntity>) query;
		}

		public override IOrderedEnumerable<TEntity> ThenBy(IOrderedEnumerable<TEntity> query)
		{
			return query;
		}
	}
}
