using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Isap.CommonCore.EntityFrameworkCore.Extensions
{
	public static class ResultSetExtensions
	{
		public static async Task<ResultSet<T>> ToResultSetAsync<T>(this IQueryable<T> data, int pageNumber, int pageSize, int? totalCount = null)
		{
			List<T> pageData = await data.Skip((pageNumber - 1) * pageSize).Take(pageSize + (pageSize == int.MaxValue ? 0 : 1)).ToListAsync();
			return new ResultSet<T>(pageData, pageNumber, pageSize, totalCount);
		}
	}
}
