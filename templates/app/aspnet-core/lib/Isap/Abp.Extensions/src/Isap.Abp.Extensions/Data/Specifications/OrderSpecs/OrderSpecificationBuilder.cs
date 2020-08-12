using System;
using System.Linq.Expressions;

namespace Isap.Abp.Extensions.Data.Specifications.OrderSpecs
{
	public class OrderSpecificationBuilder<TEntity>: SpecificationBuilderBase<TEntity, OrderSpecificationParameters>
	{
		private readonly Func<ISpecificationBuildingContext, Expression<Func<TEntity, object>>> _getExpression;

		public OrderSpecificationBuilder(Func<ISpecificationBuildingContext, Expression<Func<TEntity, object>>> getExpression)
		{
			_getExpression = getExpression;
		}

		public override ISpecification<TEntity> Create(ISpecificationBuildingContext context, OrderSpecificationParameters parameters)
		{
			return parameters.IsDescending
					? (ISpecification<TEntity>) new OrderByDescendingSpecification<TEntity>(_getExpression(context))
					: new OrderBySpecification<TEntity>(_getExpression(context))
				;
		}
	}
}
