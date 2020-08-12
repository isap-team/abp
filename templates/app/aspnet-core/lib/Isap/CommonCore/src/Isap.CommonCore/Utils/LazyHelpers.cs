using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Isap.CommonCore.Utils
{
	public static class LazyHelpers
	{
		public static IList<TImpl> CreateLazyCollection<TIntf, TImpl>(this IList<TImpl> collection, out Lazy<IList<TIntf>> lazyCollection)
			where TImpl: TIntf
		{
			lazyCollection = new Lazy<IList<TIntf>>(() => collection == null ? null : new ReadOnlyCollection<TIntf>(collection.Cast<TIntf>().ToList()));
			return collection;
		}

		public static List<TImpl> CreateLazyCollection<TIntf, TImpl>(this List<TImpl> collection, out Lazy<IList<TIntf>> lazyCollection)
			where TImpl: TIntf
		{
			lazyCollection = new Lazy<IList<TIntf>>(() => collection == null ? null : new ReadOnlyCollection<TIntf>(collection.Cast<TIntf>().ToList()));
			return collection;
		}
	}
}
