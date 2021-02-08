using System;
using System.Linq.Expressions;

namespace Isap.Abp.Extensions.Data.Specifications.FilterSpecs.Concrete
{
	public class IgnoreMultiTenantSpecification<TEntity>: FilterSpecificationBase<TEntity>
	{
		public override bool IsIgnoreMultiTenantFilter => true;

		public override Expression<Func<TEntity, bool>> IsSatisfiedBy()
		{
			return e => true;
		}
	}
}
