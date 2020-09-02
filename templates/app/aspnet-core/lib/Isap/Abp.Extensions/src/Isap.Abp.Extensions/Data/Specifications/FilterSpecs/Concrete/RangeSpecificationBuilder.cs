using System;
using System.Linq.Expressions;

namespace Isap.Abp.Extensions.Data.Specifications.FilterSpecs.Concrete
{
	public class RangeSpecificationBuilder<TEntity, TValue>: SpecificationBuilderBase<TEntity, RangeSpecificationParameters<TValue>>
	{
		private readonly Func<ISpecificationBuildingContext, RangeSpecificationParameters<TValue>, Expression<Func<TEntity, bool>>> _getExpression;

		public RangeSpecificationBuilder(
			Func<ISpecificationBuildingContext, RangeSpecificationParameters<TValue>, Expression<Func<TEntity, bool>>> getExpression)
		{
			_getExpression = getExpression;
		}

		public override ISpecification<TEntity> Create(ISpecificationBuildingContext context, RangeSpecificationParameters<TValue> parameters)
		{
			return new RangeSpecification<TEntity>(_getExpression(context, parameters));
		}
	}
}
