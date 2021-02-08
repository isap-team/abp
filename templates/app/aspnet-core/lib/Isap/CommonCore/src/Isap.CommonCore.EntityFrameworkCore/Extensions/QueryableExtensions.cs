using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Isap.CommonCore.EntityFrameworkCore.Extensions
{
	public static class QueryableExtensions
	{
		public static IQueryable<T> AsNoTracking<T>(this IQueryable<T> query, bool asNoTracking)
			where T: class
		{
			return asNoTracking ? query.AsNoTracking() : query;
		}
	}
}
