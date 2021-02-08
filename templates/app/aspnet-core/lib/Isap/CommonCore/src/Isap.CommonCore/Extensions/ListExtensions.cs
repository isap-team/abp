using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Isap.CommonCore.Extensions
{
	public static class ListExtensions
	{
		public static ReadOnlyCollection<T> ToReadOnlyCollection<T>(this IList<T> collection)
		{
			return new ReadOnlyCollection<T>(collection);
		}

		public static List<T> SplitAndRemoveAll<T>(this IList<T> collection, Func<T, bool> predicate)
		{
			List<T> result = collection.Where(predicate).ToList();
			result.ForEach(i => collection.Remove(i));
			return result;
		}
	}
}
