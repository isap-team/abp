using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Isap.CommonCore
{
	public static class ResultSetExtensions
	{
		public static ResultSet<T> ToResultSet<T>(this IEnumerable<T> data, int pageNumber, int pageSize, int? totalCount = null)
		{
			IEnumerable<T> pageData = data.Skip((pageNumber - 1) * pageSize).Take(pageSize + (pageSize == int.MaxValue ? 0 : 1));
			return new ResultSet<T>(pageData, pageNumber, pageSize, totalCount);
		}

		public static ResultSetExtensions<T> Extensions<T>(this Task<ResultSet<T>> task)
		{
			return new ResultSetExtensions<T>(task);
		}

		public static Task<ResultSet<TResult>> ConvertAsync<TSource, TResult>(this Task<ResultSet<TSource>> task, Func<TSource, TResult> convert)
		{
			return task.ContinueWith(t => t.Result.Convert(convert));
		}

		public static Task<ResultSet<TResult>> CastAsync<TSource, TResult>(this Task<ResultSet<TSource>> task)
		{
			return task.ContinueWith(t => t.Result.Convert(i => (TResult) i));
		}
	}

	public class ResultSetExtensions<T>
	{
		private readonly Task<ResultSet<T>> _task;

		public ResultSetExtensions(Task<ResultSet<T>> task)
		{
			_task = task;
		}

		public Task<ResultSet<TResult>> ConvertAsync<TResult>(Func<T, TResult> convert)
		{
			return _task.ContinueWith(t => t.Result.Convert(convert));
		}

		public Task<ResultSet<TResult>> CastAsync<TResult>()
		{
			return _task.ContinueWith(t => t.Result.Convert(i => (TResult) i));
		}
	}
}
