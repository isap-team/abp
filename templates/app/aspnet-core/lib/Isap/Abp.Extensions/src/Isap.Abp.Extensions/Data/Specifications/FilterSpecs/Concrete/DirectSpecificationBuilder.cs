using System;
using System.Linq.Expressions;

namespace Isap.Abp.Extensions.Data.Specifications.FilterSpecs.Concrete
{
	public class DirectSpecificationBuilder<TEntity, TValue>: SpecificationBuilderBase<TEntity, DirectSpecificationParameters<TValue>>
	{
		private readonly Func<ISpecificationBuildingContext, DirectSpecificationParameters<TValue>, Expression<Func<TEntity, bool>>> _getExpression;

		public DirectSpecificationBuilder(
			Func<ISpecificationBuildingContext, DirectSpecificationParameters<TValue>, Expression<Func<TEntity, bool>>> getExpression)
		{
			_getExpression = getExpression;
		}

		public override ISpecification<TEntity> Create(ISpecificationBuildingContext context, DirectSpecificationParameters<TValue> parameters)
		{
			return new DirectSpecification<TEntity>(_getExpression(context, parameters));
		}
	}
}
