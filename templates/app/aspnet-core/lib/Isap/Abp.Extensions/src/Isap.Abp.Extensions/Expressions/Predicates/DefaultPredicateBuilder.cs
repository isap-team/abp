using System;
using System.Linq.Expressions;

namespace Isap.Abp.Extensions.Expressions.Predicates
{
	public sealed class DefaultPredicateBuilder: PredicateBuilderBase
	{
		private DefaultPredicateBuilder()
		{
		}

		public static IPredicateBuilder Instance { get; } = new DefaultPredicateBuilder();

		public override Expression<Func<T, bool>> ILike<T>(Expression<Func<T, string>> propertyExpression, object value)
		{
			throw new NotSupportedException();
		}

		public override Expression<Func<T, bool>> ILike<T>(string propertyName, object value)
		{
			throw new NotSupportedException();
		}
	}
}
