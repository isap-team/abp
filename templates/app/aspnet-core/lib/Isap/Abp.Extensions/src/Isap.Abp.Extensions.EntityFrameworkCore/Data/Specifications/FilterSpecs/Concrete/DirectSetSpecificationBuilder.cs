using System;
using System.Linq.Expressions;

namespace Isap.Abp.Extensions.Data.Specifications.FilterSpecs.Concrete
{
	public class DirectSetSpecificationBuilder<TEntity, TValue>: SpecificationBuilderBase<TEntity, DirectSetSpecificationParameters<TValue>>
	{
		private readonly Func<ISpecificationBuildingContext, DirectSetSpecificationParameters<TValue>, Expression<Func<TEntity, bool>>> _getExpression;

		public DirectSetSpecificationBuilder(
			Func<ISpecificationBuildingContext, DirectSetSpecificationParameters<TValue>, Expression<Func<TEntity, bool>>> getExpression)
		{
			_getExpression = getExpression;
		}

		public override ISpecification<TEntity> Create(ISpecificationBuildingContext context, DirectSetSpecificationParameters<TValue> parameters)
		{
			return new DirectSpecification<TEntity>(_getExpression(context, parameters));
		}
	}
}
