using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Isap.Abp.Extensions.Data.Specifications.FilterSpecs.Concrete
{
	public class DirectSpecificationParameters<TValue>
	{
		public DirectSpecificationParameters()
		{
		}

		public DirectSpecificationParameters(TValue value)
		{
			Value = value;
		}

		public TValue Value { get; set; }
	}

	public class DirectSetSpecificationParameters<TValue>
	{
		public DirectSetSpecificationParameters()
		{
		}

		public DirectSetSpecificationParameters(ICollection<TValue> values)
		{
			Values = values;
		}

		public ICollection<TValue> Values { get; set; }
	}

	public class DirectSpecification<TEntity>: FilterSpecificationBase<TEntity>
	{
		private readonly Expression<Func<TEntity, bool>> _expression;

		public DirectSpecification(Expression<Func<TEntity, bool>> expression)
		{
			_expression = expression;
		}

		public override Expression<Func<TEntity, bool>> IsSatisfiedBy()
		{
			return _expression;
		}
	}
}
