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
	}
}
