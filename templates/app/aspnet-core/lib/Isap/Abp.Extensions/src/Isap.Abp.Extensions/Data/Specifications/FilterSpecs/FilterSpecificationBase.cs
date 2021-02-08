using System;
using System.Linq.Expressions;

namespace Isap.Abp.Extensions.Data.Specifications.FilterSpecs
{
	public abstract class FilterSpecificationBase<TEntity>: IFilterSpecification<TEntity>
	{
		public virtual bool IsIgnoreSafeDeleteFilter => false;
		public virtual bool IsIgnoreMultiTenantFilter => false;

		public abstract Expression<Func<TEntity, bool>> IsSatisfiedBy();

		Expression IFilterSpecification.IsSatisfiedBy() => IsSatisfiedBy();
	}
}
