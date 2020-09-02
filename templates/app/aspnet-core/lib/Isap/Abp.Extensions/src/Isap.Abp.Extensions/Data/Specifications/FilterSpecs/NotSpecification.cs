using System;
using System.Linq.Expressions;
using Isap.Abp.Extensions.Expressions.Predicates;

namespace Isap.Abp.Extensions.Data.Specifications.FilterSpecs
{
	public class NotSpecification<TEntity>: FilterSpecificationBase<TEntity>
	{
		private readonly IFilterSpecification<TEntity> _specification;

		public NotSpecification(IFilterSpecification<TEntity> specification)
		{
			_specification = specification;
		}

		public override bool IsIgnoreSafeDeleteFilter => _specification.IsIgnoreSafeDeleteFilter;
		public override bool IsIgnoreMultiTenantFilter => _specification.IsIgnoreMultiTenantFilter;

		public override Expression<Func<TEntity, bool>> IsSatisfiedBy()
		{
			return _specification.IsSatisfiedBy().Not();
		}
	}
}
