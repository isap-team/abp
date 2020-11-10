using System;
using System.Linq;
using System.Linq.Expressions;
using Isap.Abp.Extensions.Expressions.Predicates;
using JetBrains.Annotations;

namespace Isap.Abp.Extensions.Data.Specifications.FilterSpecs
{
	public class AndSpecification<TEntity>: FilterSpecificationBase<TEntity>
	{
		private readonly IFilterSpecification<TEntity>[] _specifications;

		public AndSpecification([NotNull] params IFilterSpecification<TEntity>[] specifications)
		{
			if (specifications == null)
				throw new ArgumentNullException(nameof(specifications));
			if (specifications.Length == 0)
				throw new ArgumentException("Parameter should not be empty.", nameof(specifications));
			_specifications = specifications;
		}

		public override bool IsIgnoreSafeDeleteFilter => _specifications?.Any(s => s.IsIgnoreSafeDeleteFilter) ?? base.IsIgnoreSafeDeleteFilter;
		public override bool IsIgnoreMultiTenantFilter => _specifications?.Any(s => s.IsIgnoreMultiTenantFilter) ?? base.IsIgnoreMultiTenantFilter;

		public override Expression<Func<TEntity, bool>> IsSatisfiedBy()
		{
			Expression<Func<TEntity, bool>> result = _specifications.First().IsSatisfiedBy();
			result = _specifications.Skip(1).Aggregate(result, (expression, specification) => expression.AndAlso(specification.IsSatisfiedBy()));
			return result;
		}
	}
}
