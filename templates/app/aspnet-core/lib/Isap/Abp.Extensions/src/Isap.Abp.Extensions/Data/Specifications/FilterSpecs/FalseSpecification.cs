using System;
using System.Linq.Expressions;
using Isap.Abp.Extensions.Expressions.Predicates;

namespace Isap.Abp.Extensions.Data.Specifications.FilterSpecs
{
	public class FalseSpecification<TEntity>: FilterSpecificationBase<TEntity>
	{
		public override Expression<Func<TEntity, bool>> IsSatisfiedBy()
		{
			return PredicateExtensions.False<TEntity>();
		}
	}
}
