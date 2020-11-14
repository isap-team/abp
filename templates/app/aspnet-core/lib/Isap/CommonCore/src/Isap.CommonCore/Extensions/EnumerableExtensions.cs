using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Isap.CommonCore.Extensions
{
	public static class EnumerableExtensions
	{
		public static IEnumerable<TResult> FullOuterJoin<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer,
			IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector,
			Func<TKey, TOuter, TInner, TResult> resultSelector, IEqualityComparer<TKey> comparer)
		{
			ILookup<TKey, TInner> innerLookup = inner.ToLookup(innerKeySelector, comparer);

			var joined = new List<TKey>();
			foreach (TOuter outerItem in outer)
			{
				TKey key = outerKeySelector(outerItem);
				if (innerLookup.Contains(key))
				{
					joined.Add(key);
					foreach (TInner innerItem in innerLookup[key])
						yield return resultSelector(key, outerItem, innerItem);
				}
				else
					yield return resultSelector(key, outerItem, default(TInner));
			}

			foreach (IGrouping<TKey, TInner> grouping in innerLookup)
			{
				if (!joined.Contains(grouping.Key, comparer))
				{
					foreach (TInner innerItem in grouping)
						yield return resultSelector(grouping.Key, default(TOuter), innerItem);
				}
			}
		}

		public static IEnumerable<TResult> FullOuterJoin<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer,
			IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector,
			Func<TKey, TOuter, TInner, TResult> resultSelector)
		{
			return FullOuterJoin(outer, inner, outerKeySelector, innerKeySelector, resultSelector, EqualityComparer<TKey>.Default);
		}

		public static int IndexOf<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			using (IEnumerator<TSource> enumerator = source.GetEnumerator())
			{
				int result = 0;
				while (enumerator.MoveNext())
				{
					if (predicate(enumerator.Current))
						return result;
					result++;
				}

				return -1;
			}
		}

		public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
		{
			foreach (T item in source)
				action(item);
		}

		public static IEnumerable<IList> ChunkBy(this IEnumerable collection, int chunkSize)
		{
			if (chunkSize <= 0)
				throw new ArgumentOutOfRangeException(nameof(chunkSize), "Chunk size should be greater then zero.");

			IEnumerator enumerator = collection.GetEnumerator();
			while (true)
			{
				List<object> tempChunk = new List<object>();
				while (tempChunk.Count < chunkSize && enumerator.MoveNext())
				{
					tempChunk.Add(enumerator.Current);
				}

				if (tempChunk.Count > 0)
					yield return tempChunk;
				else
					yield break;
			}
		}

		public static IEnumerable<IList<T>> ChunkBy<T>(this IEnumerable<T> collection, int chunkSize)
		{
			if (chunkSize <= 0)
				throw new ArgumentOutOfRangeException(nameof(chunkSize), "Chunk size should be greater then zero.");

			using (IEnumerator<T> enumerator = collection.GetEnumerator())
			{
				while (true)
				{
					List<T> tempChunk = new List<T>();
					while (tempChunk.Count < chunkSize && enumerator.MoveNext())
					{
						tempChunk.Add(enumerator.Current);
					}

					if (tempChunk.Count > 0)
						yield return tempChunk;
					else
						yield break;
				}
			}
		}

		public static ReadOnlyCollection<T> ToReadOnlyCollection<T>(this IEnumerable<T> collection)
		{
			return new ReadOnlyCollection<T>(collection.ToList());
		}

		public static ICollection<T> AsCollection<T>(this IEnumerable<T> source)
		{
			if (source == null)
				return null;
			return source as ICollection<T> ?? source.ToList();
		}

		public static IEnumerable<T> Include<T>(this IEnumerable<T> source, params T[] items)
		{
			return source == null ? items : source.Concat(items);
		}
	}
}
