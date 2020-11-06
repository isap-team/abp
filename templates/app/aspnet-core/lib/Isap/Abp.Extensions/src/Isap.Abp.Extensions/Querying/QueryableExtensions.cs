using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

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
		public static IQueryable<T> OrderBy<T>(this IQueryable<T> query, ICollection<SortOption> sortOptions)
		{
			return query.OrderBy(new SortOptionsSortProvider<T>(sortOptions));
		}
	}
}
