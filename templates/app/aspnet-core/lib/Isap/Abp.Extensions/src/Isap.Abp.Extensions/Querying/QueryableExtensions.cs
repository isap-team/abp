using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Isap.Abp.Extensions.Expressions.Predicates;

namespace Isap.Abp.Extensions.Querying
{
	public static class QueryableExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IQueryable<T> OrderBy<T>(this IQueryable<T> query, IQueryableSortProvider<T> sortProvider)
		{
			return sortProvider.Apply(query);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IQueryable<T> OrderBy<T>(this IQueryable<T> query, IPredicateBuilder predicateBuilder, ICollection<SortOption> sortOptions)
		{
			return query.OrderBy(new SortOptionsSortProvider<T>(predicateBuilder, sortOptions));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<T> OrderBy<T>(this IEnumerable<T> query, IEnumerableSortProvider<T> sortProvider)
		{
			return sortProvider.Apply(query);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<T> OrderBy<T>(this IEnumerable<T> query, IPredicateBuilder predicateBuilder, ICollection<SortOption> sortOptions)
		{
			return query.OrderBy(new SortOptionsSortProvider<T>(predicateBuilder, sortOptions));
		}
	}
}
