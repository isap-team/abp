using System;
using System.Linq.Expressions;

namespace Isap.CommonCore.Expressions.Evaluation
{
	public class PropertyGetAccessor<TEntity, TPropertyValue>: IPropertyGetAccessor<TEntity, TPropertyValue>
	{
		private readonly Func<TEntity, TPropertyValue> _getValue;

		public PropertyGetAccessor(Expression<Func<TEntity, TPropertyValue>> expression)
		{
			Expression = expression;
			PropertyName = expression.Name;
			_getValue = Expression.Compile();
		}

		public Expression<Func<TEntity, TPropertyValue>> Expression { get; }

		public string PropertyName { get; }

		public TPropertyValue GetValue(TEntity obj)
		{
			return _getValue(obj);
		}

		object IPropertyGetAccessor<TEntity>.GetValue(TEntity obj)
		{
			return GetValue(obj);
		}

		object IPropertyGetAccessor.GetValue(object obj)
		{
			return GetValue((TEntity) obj);
		}
	}
}
