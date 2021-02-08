using System;
using System.Linq.Expressions;
using Isap.Abp.Extensions.Data.Specifications.OrderSpecs;

namespace Isap.Abp.Extensions.Data.Specifications
{
	public static class SpecificationBuilder
	{
		public static IOrderSpecification<TEntity> OrderBy<TEntity>(Expression<Func<TEntity, object>> expression)
		{
			return new OrderBySpecification<TEntity>(expression);
		}

		public static IOrderSpecification<TEntity> OrderByDescending<TEntity>(Expression<Func<TEntity, object>> expression)
		{
			return new OrderByDescendingSpecification<TEntity>(expression);
		}

		public static IOrderSpecification<TEntity> ThenBy<TEntity>(this IOrderSpecification<TEntity> specification,
			Expression<Func<TEntity, object>> expression)
		{
			IOrderSpecification<TEntity> newSpecification = new OrderBySpecification<TEntity>(expression);

			if (specification is ConsolidatedOrderBySpecification<TEntity> consolidatedSpec)
			{
				consolidatedSpec.Add(newSpecification);
				return consolidatedSpec;
			}

			return new ConsolidatedOrderBySpecification<TEntity>(new[] { specification, newSpecification });
		}

		public static IOrderSpecification<TEntity> ThenByDescending<TEntity>(this IOrderSpecification<TEntity> specification,
			Expression<Func<TEntity, object>> expression)
		{
			IOrderSpecification<TEntity> newSpecification = new OrderByDescendingSpecification<TEntity>(expression);

			if (specification is ConsolidatedOrderBySpecification<TEntity> consolidatedSpec)
			{
				consolidatedSpec.Add(newSpecification);
				return consolidatedSpec;
			}

			return new ConsolidatedOrderBySpecification<TEntity>(new[] { specification, newSpecification });
		}
	}
}
