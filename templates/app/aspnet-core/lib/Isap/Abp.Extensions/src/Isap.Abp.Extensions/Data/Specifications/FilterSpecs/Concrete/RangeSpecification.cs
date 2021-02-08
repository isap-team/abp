using System;
using System.Linq.Expressions;

namespace Isap.Abp.Extensions.Data.Specifications.FilterSpecs.Concrete
{
	public class RangeSpecificationParameters<TValue>
	{
		public RangeSpecificationParameters()
		{
		}

		public RangeSpecificationParameters(TValue fromValue, TValue toValue)
		{
			FromValue = fromValue;
			ToValue = toValue;
		}

		public TValue FromValue { get; set; }
		public TValue ToValue { get; set; }
	}

	public class RangeSpecification<TEntity>: DirectSpecification<TEntity>
	{
		public RangeSpecification(Expression<Func<TEntity, bool>> expression)
			: base(expression)
		{
		}
	}
}
