using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Isap.CommonCore.Extensions
{
	public static class ListExtensions
	{
		public static ReadOnlyCollection<T> ToReadOnlyCollection<T>(this IList<T> collection)
		{
			return new ReadOnlyCollection<T>(collection);
		}
	}
}
