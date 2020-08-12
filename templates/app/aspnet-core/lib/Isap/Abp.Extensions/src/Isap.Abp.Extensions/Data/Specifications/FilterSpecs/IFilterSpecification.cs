using System;
using System.Linq.Expressions;

namespace Isap.Abp.Extensions.Data.Specifications.FilterSpecs
{
	public interface IFilterSpecification: ISpecification
	{
		bool IsIgnoreSafeDeleteFilter { get; }
		bool IsIgnoreMultiTenantFilter { get; }

		Expression IsSatisfiedBy();
	}

	public interface IFilterSpecification<TEntity>: ISpecification<TEntity>, IFilterSpecification
	{
		new Expression<Func<TEntity, bool>> IsSatisfiedBy();
	}
}
