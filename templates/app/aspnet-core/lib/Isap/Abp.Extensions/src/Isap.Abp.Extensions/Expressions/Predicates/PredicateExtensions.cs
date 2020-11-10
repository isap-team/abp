using System;
using System.Linq;
using System.Linq.Expressions;

namespace Isap.Abp.Extensions.Expressions.Predicates
{
	public static class PredicateExtensions
	{
		/// <summary>
		///     Creates a predicate that evaluates to true.
		/// </summary>
		public static Expression<Func<T, bool>> True<T>()
		{
			return param => true;
		}

		/// <summary>
		///     Creates a predicate that evaluates to false.
		/// </summary>
		public static Expression<Func<T, bool>> False<T>()
		{
			return param => false;
		}

		/// <summary>
		///     Creates a predicate expression from the specified lambda expression.
		/// </summary>
		public static Expression<Func<T, bool>> Create<T>(Expression<Func<T, bool>> predicate)
		{
			return predicate;
		}

		/// <summary>
		///     Combines the first predicate with the second using the logical "and".
		/// </summary>
		[Obsolete("Use AndAlso")]
		public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
		{
			return AndAlso(first, second);
		}

		/// <summary>
		///     Combines the first predicate with the second using the logical "and".
		/// </summary>
		public static Expression<Func<T, bool>> AndAlso<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
		{
			return first.Compose(second, Expression.AndAlso);
		}

		/// <summary>
		///     Combines the first predicate with the second using the logical "or".
		/// </summary>
		[Obsolete("Use OrElse")]
		public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
		{
			return OrElse(first, second);
		}

		/// <summary>
		///     Combines the first predicate with the second using the logical "or".
		/// </summary>
		public static Expression<Func<T, bool>> OrElse<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
		{
			return first.Compose(second, Expression.OrElse);
		}

		/// <summary>
		///     Negates the predicate.
		/// </summary>
		public static Expression<Func<T, bool>> Not<T>(this Expression<Func<T, bool>> expression)
		{
			var negated = Expression.Not(expression.Body);
			return Expression.Lambda<Func<T, bool>>(negated, expression.Parameters);
		}

		/// <summary>
		///     Combines the first expression with the second using the specified merge function.
		/// </summary>
		static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge)
		{
			// zip parameters (map from parameters of second to parameters of first)
			var map = first.Parameters
				.Select((f, i) => new { f, s = second.Parameters[i] })
				.ToDictionary(p => p.s, p => p.f);

			// replace parameters in the second lambda expression with the parameters in the first
			var secondBody = ParameterUpdateVisitor.ReplaceParameters(map, second.Body);

			// create a merged lambda expression with parameters from the first expression
			return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
		}
	}
}
