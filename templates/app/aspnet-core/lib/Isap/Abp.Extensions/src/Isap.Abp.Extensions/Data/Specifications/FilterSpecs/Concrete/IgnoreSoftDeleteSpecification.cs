using System;
using System.Linq.Expressions;

namespace Isap.Abp.Extensions.Data.Specifications.FilterSpecs.Concrete
{
	public class IgnoreSoftDeleteSpecification<TEntity>: FilterSpecificationBase<TEntity>
	{
		public override bool IsIgnoreSafeDeleteFilter => true;

		public override Expression<Func<TEntity, bool>> IsSatisfiedBy()
		{
			return e => true;
		}
	}
}
