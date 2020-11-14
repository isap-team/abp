using System;
using System.Linq;
using System.Linq.Expressions;

namespace Isap.Abp.Extensions.Data.Specifications.OrderSpecs
{
	public class OrderBySpecification<TEntity>: OrderSpecificationBase<TEntity>
	{
		private readonly Expression<Func<TEntity, object>> _expression;

		public OrderBySpecification(Expression<Func<TEntity, object>> expression)
		{
			_expression = expression;
		}

		public override bool IsEmpty => false;

		public override IOrderedQueryable<TEntity> OrderBy(IQueryable<TEntity> query)
		{
			return query.OrderBy(_expression);
		}

		public override IOrderedQueryable<TEntity> ThenBy(IOrderedQueryable<TEntity> query)
		{
			return query.ThenBy(_expression);
		}
	}
}
