using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Isap.CommonCore.Extensions
{
	public static class ObjectExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T With<T>(this T obj, Action<T> action)
		{
			action(obj);
			return obj;
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TValue As<TObject, TValue>(this TObject obj, Func<TObject, TValue> select)
		{
			return select(obj);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T[] AsArray<T>(this T item)
		{
			return Equals(item, default(T)) ? new T[0] : new[] { item };
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static List<T> AsList<T>(this T item)
		{
			return Equals(item, default(T)) ? new List<T>() : new List<T> { item };
		}
	}
}
